#ifndef _ROOM_H
#define _ROOM_H


#include <WS2tcpip.h>
#include <list>
#include <ctime>
#include <iomanip>
#include <chrono>
#include <random>

#include "CreateObjectInfo.pb.h"
#include "UpdateInfo.pb.h"


enum class Protocal {
	MESSAGE_CREATEOBJ = 1000,
	MESSAGE_UPDATEDATA = 1001,
	MESSAGE_REFLECTDATA = 1002,
	MESSAGE_DAMAGE = 1003,
	MESSAGE_INITENERGYSPHERE = 1004,
	MESSAGE_COLLECT = 1005,
	MESSAGE_GENERATORENERGY = 1006
};


enum class SphereType {
	SPHERE_RED = 1,
	SPHERE_BLUE = 2,
	SPHERE_YELLOW = 3
};



class Room {
public:
	static void CreateRoom(std::list<SOCKET> UserList, int64_t EndTime);
	static void InitEnergyShpere(std::list<SOCKET>& UserList);
	static void Encode(Protocal message_type, int size, char* buffer);
	static void CreateObject(SOCKET socket, int index);
	static const uint32_t RoomSize = 1;
	static const uint32_t SphereSize = 5;
	static const int width = 1920;
	static const int height = 1080;
};




#endif