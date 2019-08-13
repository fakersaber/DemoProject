#include "Room.h"



void Room::Encode(Protocal message_type, int size, char* buffer) {
	*reinterpret_cast<int*>(buffer) = static_cast<int>(message_type);
	*reinterpret_cast<int*>(buffer + sizeof(int)) = size;

}

void Room::CreateObject(SOCKET socket, int index) {

	char send_buffer[1024] = { 0 };
	int totalSize = 0;
	CreateObjInfo SendClass;
	for (int i = 0; i < Room::RoomSize; ++i) {
		//YVector2 position;
		//position.set_x(i * 5.f);
		//position.set_y(i * 5.f);
		SendClass.mutable_position()->set_x(i * 5.f);
		SendClass.mutable_position()->set_y(i * 5.f);
		SendClass.set_playerid(i + 1);
		SendClass.set_rotation(45.f);
		index == i + 1 ? SendClass.set_ismanclient(true) : SendClass.set_ismanclient(false);
		SendClass.SerializeToArray(send_buffer + 2 * sizeof(int) + totalSize, SendClass.ByteSize());
		Room::Encode(Protocal::MESSAGE_CREATEOBJ, SendClass.ByteSize(), send_buffer + totalSize);
		totalSize = totalSize + SendClass.ByteSize() + 2 * sizeof(int);
	}
	send(socket, send_buffer, totalSize, 0);
}


void Room::InitEnergyShpere(std::list<SOCKET>& UserList) {

	char send_buffer[1024] = { 0 };
	std::random_device rd;
	std::mt19937 gen(rd());
	std::uniform_int_distribution<> width(-Room::width, Room::width);
	std::uniform_int_distribution<> height(-Room::height, Room::height);

	EnergySphereInit AllSphereInfo;
	int32_t SphereID = 1;
	for (int i = 0; i < Room::SphereSize; ++i) {
		auto GeneratorSphere = AllSphereInfo.add_allspherepoll();
		GeneratorSphere->set_type(static_cast<int>(SphereType::SPHERE_RED));
		GeneratorSphere->set_sphereid(SphereID++);
		GeneratorSphere->mutable_position()->set_x(width(gen)* 0.01f);
		GeneratorSphere->mutable_position()->set_y(height(gen)* 0.01f);

		GeneratorSphere = AllSphereInfo.add_allspherepoll();
		GeneratorSphere->set_type(static_cast<int>(SphereType::SPHERE_BLUE));
		GeneratorSphere->set_sphereid(SphereID++);
		GeneratorSphere->mutable_position()->set_x(width(gen)* 0.01f);
		GeneratorSphere->mutable_position()->set_y(height(gen)* 0.01f);

		GeneratorSphere = AllSphereInfo.add_allspherepoll();
		GeneratorSphere->set_type(static_cast<int>(SphereType::SPHERE_YELLOW));
		GeneratorSphere->set_sphereid(SphereID++);
		GeneratorSphere->mutable_position()->set_x(width(gen)* 0.01f);
		GeneratorSphere->mutable_position()->set_y(height(gen)* 0.01f);
	}
	AllSphereInfo.SerializeToArray(send_buffer + 2 * sizeof(int), AllSphereInfo.ByteSize());
	Room::Encode(Protocal::MESSAGE_INITENERGYSPHERE, AllSphereInfo.ByteSize(), send_buffer);

	for (auto client : UserList) {
		send(client, send_buffer, AllSphereInfo.ByteSize() + 2 * sizeof(int), 0);
	}


}

void Room::CreateRoom(std::list<SOCKET> UserList, int64_t EndTime) {
	//初始化能量球以及玩家
	InitEnergyShpere(UserList);
	int index = 1;
	for (auto iter = UserList.begin(); iter != UserList.end(); ++iter, ++index) {
		CreateObject(*iter, index);
	}
	//select模型  
	fd_set AllSocketSet;
	fd_set ReadFd;
	FD_ZERO(&AllSocketSet);
	for (auto UserSock : UserList)
		FD_SET(UserSock, &AllSocketSet);
	char RevData[4096] = { 0 };
	int RetVal = 0;
	while (true) {
		ReadFd = AllSocketSet;
		RetVal = select(0, &ReadFd, NULL, NULL, NULL);
		if (RetVal == SOCKET_ERROR) {
			printf("create Room error or room end\n");
			return;
		}
		for (uint32_t i = 0; i < AllSocketSet.fd_count; ++i) {
			//有数据可读
			if (FD_ISSET(AllSocketSet.fd_array[i], &ReadFd)) {
				RetVal = recv(AllSocketSet.fd_array[i], RevData, 4096, 0);
				//连接异常
				{
					if (RetVal == SOCKET_ERROR || !RetVal) {
						closesocket(AllSocketSet.fd_array[i]);
						FD_CLR(AllSocketSet.fd_array[i], &AllSocketSet);

						if (!RetVal) { printf("Client closes normally\n"); }
						else {
							auto err = WSAGetLastError();
							if (err == WSAECONNRESET) { printf("Client is forced to close\n"); }
							else { printf("unkown error %d \n", err); }
						}
						continue;
					}
				}
				for (auto client : UserList) {
					if (client != AllSocketSet.fd_array[i]) {
						send(client, RevData, RetVal, 0);
					}
				}

			}
		}
	}
}

//if (*reinterpret_cast<int*>(RevData) == static_cast<int>(Protocal::MESSAGE_GENERATORENERGY)) {
//	EnergySphere NewSphere;
//	uint32_t offset = 0;
//	while (RetVal != 0) {
//		int realsize = *reinterpret_cast<int*>(RevData + offset + sizeof(int));
//		NewSphere.ParseFromArray(RevData + sizeof(int) * 2 + offset, realsize);
//		NewSphere.mutable_position()->set_x(width(gen)* 0.01f);
//		NewSphere.mutable_position()->set_y(height(gen)* 0.01f);
//		RetVal = RetVal - realsize - 2 * sizeof(int);
//		offset = offset + realsize + sizeof(int);
//		NewSphere.SerializeToArray(SendBuffer + sizeof(int) * 2, NewSphere.ByteSize());
//		Room::Encode(Protocal::MESSAGE_GENERATORENERGY, NewSphere.ByteSize(), SendBuffer);
//		for (auto client : UserList) {
//			if (client != AllSocketSet.fd_array[i]) {
//				send(client, SendBuffer, NewSphere.ByteSize() + 2 * sizeof(int), 0);
//			}
//		}
//	}
//}
//else {
	//开始同步数据，仅仅转发

