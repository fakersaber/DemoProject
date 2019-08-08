#include "WinServer.h"


int main(int argc, char* argv[])
{
	WinServer Server;
	if (Server.init()) {
		Server.Run();
	}
	return 0;
}