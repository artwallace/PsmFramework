/*
	Gamepad server for PSS
	By ultrano#ho tma il#com

	Requires a connected gamepad to the PC, and preferably it's a dualshock2 with PS2->USB converter,
	or a dualshock3 (didn't try with the latter).

	Supplies more buttons than the Vita, for more freedom of control when developing. Extend on this when you need.
*/

#pragma comment(lib, "Winmm.lib")
#pragma comment(lib, "Ws2_32.lib")

#include <WinSock2.h>
#include <windows.h>
#include <stdio.h>

bool exitserver=false;
SOCKET server;

typedef unsigned char U8;
typedef unsigned int  U32;

struct vec2{
	float x,y;
};

enum ILXGPBUTTONS{
	TRIANGLE,CIRCLE,CROSS,SQUARE,L2,R2,L1,R1,SELECT,START,L3,R3,LEFT,RIGHT,UP,DOWN
};

struct PAD{
	int size;
	vec2 LStick;
	vec2 RStick;

	U32  buttons;
};

PAD ILXPad1 ={0};

static void ILXUpdateGamepad(){
	JOYINFOEX t;
	PAD j;

	memset(&t,0,sizeof(t));
	memset(&j,0,sizeof(j));
	t.dwSize=sizeof(JOYINFOEX);
	t.dwFlags=JOY_RETURNALL;

	if(JOYERR_NOERROR==joyGetPosEx(JOYSTICKID1,&t)){
		j.size = sizeof(j);

		j.LStick.x = (t.dwXpos - 32767.0f)/32767.0f;
		j.LStick.y = (t.dwYpos - 32767.0f)/32767.0f;
		j.RStick.x = (t.dwRpos - 32767.0f)/32767.0f;
		j.RStick.y = (t.dwZpos - 32767.0f)/32767.0f;

		j.buttons = t.dwButtons;

		#define fixup(what) if(what> -0.2f && what<0.2f)what=0;  else if(what<-1.0f)what= -1.0f; else if(what>1.0f)what= 1.0f;
		fixup(j.LStick.x);
		fixup(j.LStick.y);
		fixup(j.RStick.x);
		fixup(j.RStick.y);
		#undef fixup

		if(t.dwPOV==27000)j.buttons |= 1 << LEFT;
		if(t.dwPOV==9000 )j.buttons |= 1 << RIGHT;
		if(t.dwPOV==0    )j.buttons |= 1 << UP;
		if(t.dwPOV==18000)j.buttons |= 1 << DOWN;
	}

#if 0
	for(int i=0;i<16;i++){
		U8 b1,b2,b3;
		b1 = ILXPad1.buttons[i];
		b2 = j.buttons[i];
		b3 = ILXPad1.repeat[i];
		if(b1 && b2){
			if(b3<255)b3++;
			j.repeat[i]=b3;
		}
		j.newbuttons[i] = (b1^b2)&b2;
		j.buttons[i]=b2;
	}
#endif

	memcpy(&ILXPad1,&j,sizeof(j));
}

DWORD WINAPI ServerThread(LPVOID pParam)
{
	WSADATA wsaData;
	sockaddr_in local;

	int wsaret=WSAStartup(0x101,&wsaData);
	if(wsaret!=0){
        return 0;
    }

	local.sin_family=AF_INET; //Address family
    local.sin_addr.s_addr=INADDR_ANY; //Wild card IP address
    local.sin_port=htons((u_short)1777); //port to use
    server=socket(AF_INET,SOCK_STREAM,0);
    if(server==INVALID_SOCKET)
    {
        return 0;
    }
    if(bind(server,(sockaddr*)&local,sizeof(local))!=0)
    {
        return 0;
    }
    if(listen(server,10)!=0)
    {
        return 0;
    }

    SOCKET client;
    sockaddr_in from;
    int fromlen=sizeof(from);

    printf("server started\n");

    fflush(stdout);
    while(!exitserver)
    {
        char indata;

        client=accept(server, (struct sockaddr*)&from,&fromlen);
		printf("client connected. Press q to exit, s to stop\n");
		fflush(stdout);

		for(;;){
			indata = 0;
			recv(client,&indata,1,0);
			if(!indata)break;
			if(indata=='s')break;
			if(indata=='q'){ exitserver=true; break;}
			ILXUpdateGamepad();

			send(client,(char*)&ILXPad1,sizeof(ILXPad1),0);
		}
        closesocket(client);
        printf("client gone\n");
		fflush(stdout);
    }

	return 0;
}

int main(){
	printf("Starting server at port 1777...\n");

#if 0
	DWORD tid;
	CreateThread(0,0,ServerThread,0,0,&tid);

	for(;;){
		printf("Press q to exit:\n");
		char c = getchar();
		if(c=='q')break;
	}
#else
	ServerThread(0);
#endif

	exitserver = true;
	closesocket(server);
	WSACleanup();

	return 0;
}
