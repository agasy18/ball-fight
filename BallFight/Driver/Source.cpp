#include <Windows.h>

const char* commands[]={
	"taskkill /im BallServer.exe",
	"..\\..\\..\\BallServer\\bin\\Debug\\BallServer.exe"
	"exit"
};

int main()
{
	for (auto command:commands)
	{
		system(command);
	}
	return 0;
}