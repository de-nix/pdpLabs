#include <list>
#include <condition_variable>
#include <thread>

using namespace std;

template <typename T>
class ProducerConsumerQueue {
    list<T> items;
    condition_variable cv;
    mutex mtx;
public:
    void enqueue(T v) {
        items.push_back(v);
        cv.notify_one();
    }
    T dequeue() {
        {
            unique_lock<mutex> lck(mtx);
            while (items.empty()) {
                cv.wait(lck);
            }
        }
        {
            unique_lock<mutex> lck(mtx);
            T ret = items.front();
            items.pop_front();
            return ret;
        }
    }
};