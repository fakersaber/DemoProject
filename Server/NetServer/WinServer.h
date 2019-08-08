#ifndef _WINSERVER_H
#define _WINSERVER_H

#include <WS2tcpip.h>
#include <thread>
#include "Room.h"

class WinServer {

public:
	WinServer();
	~WinServer();
	bool init();
	void Run();



private:
	SOCKET sockSrv;
	SOCKADDR_IN addrClt;
	std::list<SOCKET> UserList;
	std::list<SOCKET> RankList;

public:
	static const int DEFAULT_PORT = 8000;

};
#endif
