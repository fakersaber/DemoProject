#include "WinServer.h"



bool WinServer::init() {
	//Socket 初始化
	WORD wVersionRequested = MAKEWORD(2, 2);
	WSADATA wsaData = {0};

	auto err = WSAStartup(wVersionRequested, &wsaData);
	if (err != 0)
	{
		printf("WSAStartup failed with error: %d\n", err);
		return false;
	}
	if (LOBYTE(wsaData.wVersion) != 2 || HIBYTE(wsaData.wVersion) != 2)
	{
		printf("Could not find a usable version of Winsock.dll\n");
		WSACleanup();
		return false;
	}
	sockSrv = socket(AF_INET, SOCK_STREAM, 0);
	if (sockSrv == INVALID_SOCKET)
	{
		printf("socket() fail:%d\n", WSAGetLastError());
		return false;
	}
	SOCKADDR_IN addrSrv = {0};
	addrSrv.sin_family = AF_INET;
	addrSrv.sin_addr.s_addr = htonl(INADDR_ANY);
	addrSrv.sin_port = htons(WinServer::DEFAULT_PORT);
	err = bind(sockSrv, (SOCKADDR*)&addrSrv, sizeof(SOCKADDR));
	if (err != 0)
	{
		printf("bind()fail:%d\n", WSAGetLastError());
		return false;
	}
	err = listen(sockSrv, 64);//listen函数的第一个参数即为要监听的socket描述字，第二个参数为相应socket可以排队的最大连接个数
	if (err != 0)
	{
		printf("listen()fail:%d\n", WSAGetLastError());
		return false;
	}

	memset(&addrClt, 0, sizeof(SOCKADDR));
	printf("Server Init Success!\n");
	return true;
}


void WinServer::Run() {
	int len = sizeof(SOCKADDR);
	int recvlen = 0;
	while (true)
	{
		SOCKET sockClient = accept(sockSrv, reinterpret_cast<SOCKADDR*>(&addrClt), &len);
		RankList.emplace_back(sockClient);
		if (RankList.size() == Room::RoomSize) {
			new std::thread(std::bind(&Room::CreateRoom, RankList,0));
			RankList.clear();
		}
	}
}

WinServer::WinServer()
{
}

WinServer::~WinServer() {
	closesocket(sockSrv);
	WSACleanup();
}
