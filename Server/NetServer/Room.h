#ifndef _ROOM_H
#define _ROOM_H


#include <WS2tcpip.h>
#include <list>
#include <ctime>
#include <iomanip>
#include <chrono>

#include "CreateObjectInfo.pb.h"
#include "UpdateInfo.pb.h"


enum class Protocal {
	MESSAGE_CREATEOBJ = 1000,
	MESSAGE_UPDATEDATA = 1001,
	MESSAGE_SNEDLOCALDATA = 1002
};


class Room {
public:
	static void CreateRoom(std::list<SOCKET> UserList, int64_t EndTime);
	static void Encode(Protocal message_type, int size, char* buffer);
	static void CreateObject(SOCKET socket, int index);
	static const uint32_t RoomSize = 2;
};




#endif