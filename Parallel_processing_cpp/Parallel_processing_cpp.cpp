#include <iostream>
#include <windows.h>
#include <time.h>
#include <thread>
#include <mutex>
#include <fstream>

using namespace std;

#define n 500
int repeat = 100;

string fileName = "result.txt";

double multiplier_1[n][n];
double multiplier_2[n][n];
double product[n][n];

int threadCount = 0;
thread* hThreads;

HANDLE hStart = CreateEvent(NULL, true, false, NULL);

void ResetToZero(double(&m)[n][n])
{
	for (int i = 0; i < n; i++) {
		for (int j = 0; j < n; j++) {
			m[i][j] = 0;
		}
	}
}

void MatrixMultiplication(int index)
{
	WaitForSingleObject(hStart, INFINITE);
	for (int i = index; i < n; i += threadCount) {
		for (int j = 0; j < n; j++){
			for (int k = 0; k < n; k++) {
				product[i][j] += multiplier_1[i][k] * multiplier_2[k][j];
			}
		}
	}
}

int main() {
	srand((int)time(0));
	for (int i = 0; i < n; i++)
	{
		for (int j = 0; j < n; j++)
		{
			multiplier_1[i][j] = 10 * (double)rand() / RAND_MAX;
			multiplier_2[i][j] = 10 * (double)rand() / RAND_MAX;
		}
	}

	printf("Threads count: ");
	cin >> threadCount;

	double sumTime = 0;
	double seconds = 0;
	ofstream fout(fileName);
	for (int k = 0; k < repeat; k++)
	{
		ResetToZero(product);
		hThreads = new thread[threadCount];

		for (int i = 0; i < threadCount; i++) {
			hThreads[i] = thread(MatrixMultiplication, i);
		}
		
		clock_t start = clock();
		
		SetEvent(hStart);
		for (int i = 0; i < threadCount; i++) {
			hThreads[i].join();
		}
		
		seconds = ((double)clock() - (double)start) / 1000.0;
		sumTime += seconds;
		fout << seconds << '\n';

		delete[] hThreads;
		ResetEvent(hStart);
	}
	fout.close();
	printf("\nthreads: %d\ntime: %f", threadCount, (sumTime / repeat));
	
	char* filename = new char[fileName.length() + 1];
	strcpy(filename, fileName.c_str());
	system(filename);
	
	system("pause");
	return 0;
}