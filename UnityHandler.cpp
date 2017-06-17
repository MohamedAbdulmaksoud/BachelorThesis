#include "UnityHandler.h"
#include <iostream>
#include<WS2tcpip.h>
#include <string>
#pragma comment(lib, "ws2_32.lib")
struct unityHandler
{
	in_addr serverAddress;
	SOCKET unitySocket;
	std::string err;
	char b_sensor[256];
	char b_actuator[256];
private: void connectToUnity();
		 void writeActuatorData(char c, int i);
		 void disconnect();
private:int state; //0:unityHandler initialized 1:Winsock_initialized 2:Socket_Created 3:Socket_Connected 4:Sending_Data &Reading_Data 5:Disconnect

		unityHandler::unityHandler(in_addr address) {
			state = 0;
			serverAddress = address;
			connectToUnity();
		}
		unityHandler::~unityHandler() {
			disconnect();
		}
};


void unityHandler::connectToUnity()
{
	int port = 61499;
	//Initialize WinSock
	WSAData data;
	int wsResult = WSAStartup(MAKEWORD(2, 2), &data);
	if (wsResult != 0)
	{
		err = "Can't start Winsock: %d\n" + WSAGetLastError();
		return;
	}	
	//create socket
	SOCKET client = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (client == INVALID_SOCKET) {
		err = "Can't create socket: %d\n" + WSAGetLastError();
		WSACleanup();
		return;
	}	
	//Fill in a hint structure
	sockaddr_in hint;
	hint.sin_family = AF_INET;
	hint.sin_port = htons(port);
	hint.sin_addr = serverAddress;

	//Connect to server
	int connResult = connect(client, (sockaddr*)&hint, sizeof(hint));
	if (connResult == SOCKET_ERROR)
	{
		err = "Can't connect to server: %d\n" + WSAGetLastError();
		closesocket(client);
		WSACleanup();
		return;
	}
	
	unitySocket = client;
	
	int iResult;
	do {
		iResult = send(client, b_actuator, 256, 0);
		if (iResult == SOCKET_ERROR) {
			printf("send failed with error: %d\n", WSAGetLastError());
			closesocket(client);
			WSACleanup();
			return;
		}
		iResult = recv(client, b_sensor, 256, 0);
		if (iResult == 0)
			err = "Connection closed";
		else
			err = "recv failed with error: %d\n" + WSAGetLastError();

	} while (iResult > 0);
	disconnect();
}


void unityHandler::disconnect()
{
	closesocket(unitySocket);
	int cleanResult = WSACleanup();
	if (cleanResult != 0) {
		err = "WSACleanup FAILED: %d\n" + WSAGetLastError();
	}
}

void unityHandler::writeActuatorData(char c, int i)
{
	b_actuator[i] = c;
}
