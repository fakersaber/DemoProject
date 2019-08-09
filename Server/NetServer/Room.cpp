#include "Room.h"



void Room::Encode(Protocal message_type,int size,char* buffer) {
	*reinterpret_cast<int*>(buffer) = static_cast<int>(message_type);
	*reinterpret_cast<int*>(buffer + sizeof(int)) = size;

}

void Room::CreateObject(SOCKET socket,int index) {

	char send_buffer[1024] = { 0 };
	int totalSize = 0;
	for (int i = 0; i < Room::RoomSize; ++i) {
		CreateObjInfo SendClass;
		YVector2 position;
		position.set_x(i * 5.f);
		position.set_y(i * 5.f);
		*SendClass.mutable_position() = position;
		SendClass.set_playerid(i + 1);
		SendClass.set_rotation(45.f);
		index == i + 1 ? SendClass.set_ismanclient(true) : SendClass.set_ismanclient(false);
		SendClass.SerializeToArray(send_buffer + 2 * sizeof(int) + totalSize, SendClass.ByteSize());
		Room::Encode(Protocal::MESSAGE_CREATEOBJ, SendClass.ByteSize(), send_buffer + totalSize);
		totalSize = totalSize + SendClass.ByteSize() + 2 * sizeof(int);
	}
	send(socket, send_buffer, totalSize, 0);
}


void Room::CreateRoom(std::list<SOCKET> UserList, int64_t EndTime) {

	std::unordered_map<int, SOCKET> UserTable;

	//通知客户端创建对象
	int index = 1;
	for (auto iter = UserList.begin(); iter != UserList.end(); ++iter,++index) {
		CreateObject(*iter,index);
		UserTable.insert(std::make_pair(index, *iter));
	}


	 //select模型  
	fd_set AllSocketSet;
	fd_set ReadFd;
	fd_set WriteFd;
	FD_ZERO(&AllSocketSet);
	for (auto UserSock : UserList)
		FD_SET(UserSock, &AllSocketSet);
	char RevData[1024] = { 0 };
	int RetVal = 0;
	while (true){
		ReadFd = AllSocketSet;
		WriteFd = AllSocketSet;
		RetVal = select(0, &ReadFd, &WriteFd, NULL, NULL);
		if (RetVal == SOCKET_ERROR){
			printf("create Room error or room end\n");
			return;
		}
		for (uint32_t i = 0; i < AllSocketSet.fd_count; ++i) {
			//有数据可读
			if (FD_ISSET(AllSocketSet.fd_array[i], &ReadFd)) {
				RetVal = recv(AllSocketSet.fd_array[i], RevData, 1024, 0);

				//连接异常
				{
					if (RetVal == SOCKET_ERROR || !RetVal) {
						closesocket(AllSocketSet.fd_array[i]);
						FD_CLR(AllSocketSet.fd_array[i], &AllSocketSet);

						if (!RetVal) { printf("Client closes normally\n"); }
						else {
							auto err = WSAGetLastError();
							if (err == WSAECONNRESET) { printf("Client is forced to close\n"); }
							else { printf("unkown error\n"); }
						}
						continue;
					}
				}

				//开始同步数据，仅仅转发

				for (auto client : UserList) {
					if (client != AllSocketSet.fd_array[i]) {
						send(client, RevData, RetVal, 0);
					}
				}


				//UpdateInfo ReciveData;
				//char Sendbuffer[1024];
				//uint32_t offset = 0;
				//while (RetVal != 0) {
				//	int realsize = *reinterpret_cast<int*>(RevData + offset);
				//	ReciveData.ParseFromArray(RevData + sizeof(int) + offset, realsize);
				//	RetVal = RetVal - realsize - sizeof(int);
				//	offset = offset + realsize + sizeof(int);

				//	ReciveData.SerializeToArray(Sendbuffer + sizeof(int) * 2, ReciveData.ByteSize());
				//	Room::Encode(Protocal::MESSAGE_UPDATEDATA, ReciveData.ByteSize(), Sendbuffer);
				//	for (auto client : UserList) {
				//		if (client != UserTable[ReciveData.playerid()]) {
				//			send(client, Sendbuffer, sizeof(int) * 2 + ReciveData.ByteSize(), 0);
				//		}	
				//	}
				//}
			}
		}
	}
}



void UpdateTransform() {

}
