#include <iostream>
#include <fstream>
#include <unistd.h>
#include <sys/inotify.h>
#include <cstdio>

std::string readClip() {
    FILE* pipe = popen("xclip -o -selection clipboard", "r");
    char buffer[4096];
    std::string result;
    while (fgets(buffer, sizeof(buffer), pipe)) result += buffer;
    pclose(pipe);
    return result;
}

void writeClip(const std::string& text) {
    FILE* pipe = popen("xclip -i -selection clipboard", "w");
    fwrite(text.c_str(), 1, text.size(), pipe);
    pclose(pipe);
}

int main() {
    std::string path = "/media/uos/.clipboard/clip.dat";
    std::string last;

    while (true) {
        if (access(path.c_str(), F_OK) == 0) {
            std::ifstream in(path);
            std::string t((std::istreambuf_iterator<char>(in)),
                           std::istreambuf_iterator<char>());
            if (!t.empty()) writeClip(t);
        }

        std::string cur = readClip();
        if (cur != last && !cur.empty()) {
            last = cur;
            std::ofstream out(path);
            out << cur;
        }

        sleep(1);
    }
}
