/* Establish a TCP socket for communication with Unity's server.
** IP address of the server will have to be imported from FORTE.
** Number of both IX/QX blocks will also affect the array size of data received/sent respectively.
*/
#include <iostream>
#include<WS2tcpip.h>
#include <string>
#pragma comment(lib, "ws2_32.lib")


void main()
{
	std::string ipAddress = "127.0.0.1";
	int port = 61499;
	//Initialize WinSock
	WSAData dataS;
	WORD ver = MAKEWORD(2, 2);
	int wsResult = WSAStartup(ver, &dataS);
	if (wsResult != 0)
	{
		std::cerr << "Can't start Winsock, Err #" << wsResult << std::endl;
		return;
	}
	//create socket
	SOCKET client = socket(AF_INET, SOCK_STREAM, 0);
	if (client == INVALID_SOCKET) {
		std::cerr << "Can't create socket, Err #" << WSAGetLastError() << std::endl;
		WSACleanup();
		return;
	}
	//Fill in a hint structure
	sockaddr_in hint;
	hint.sin_family = AF_INET;
	hint.sin_port = htons(port);
	inet_pton(AF_INET, ipAddress.c_str(), &hint.sin_addr);

	//Connect to server
	int connResult = connect(client, (sockaddr*)&hint, sizeof(hint));
	if (connResult == SOCKET_ERROR)
	{
		std::cerr << "Can't connect to server, Err #" << WSAGetLastError() << std::endl;
		closesocket(client);
		WSACleanup();
		return;
	}
	//Send and receive data
	char data[256];
	// until the peer closes the connection
	int iResult;
	do {
		data[0] = rand() % 2 == 0;
		data[3] = rand() % 2 == 0;
		data[6] = rand() % 2 == 0;
		data[9] = rand() % 2 == 0;
		iResult = send(client, data, 256, 0);
		if (iResult == SOCKET_ERROR) {
			printf("send failed with error: %d\n", WSAGetLastError());
			closesocket(client);
			WSACleanup();
			return;
		}
		iResult = recv(client, data, 256, 0);
		if (iResult > 0)
			printf("Bytes received: %d\n", iResult);
		else if (iResult == 0)
			printf("Connection closed\n");
		else
			printf("recv failed with error: %d\n", WSAGetLastError());

	} while (iResult > 0);

	// cleanup
	closesocket(client);
	WSACleanup();
	return;

}
