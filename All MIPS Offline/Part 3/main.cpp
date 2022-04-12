#include <bits/stdc++.h>
using namespace std;

#define pii 	pair<int, int>
#define opcode 	first
#define optype 	second
#define MP	make_pair

/**
 * oplist has pair of (<operation_name>, <operation_type>)
 * operation_type can be of 3 types
 *      R -> 0
 *      I -> 1
 *      J -> 2
*/
vector<pair<string, int>> opList{
        MP("add", 0),
        MP("addi", 1),
        MP("sub", 0),
        MP("subi", 1),
        MP("and", 0),
        MP("andi", 1),
        MP("or", 0),
        MP("ori", 1),
        MP("sll", 0),
        MP("srl", 0),
        MP("nor", 0),
        MP("sw", 1),
        MP("lw", 1),
        MP("beq", 1),
        MP("bneq", 1),
        MP("j", 2)
};

map<string, int> registerList = {
        {"$t0", 0},
        {"$t1", 1},
        {"$t2", 2},
        {"$t3", 3},
        {"$t4", 4},
        {"$zero", 5}
};

/**
 * Splits a string into a list of words
*/
vector<string> getWords(string str) {
        vector<string> ret;
        string word = "";
        for(int i = 0; i < str.size(); i++) {
                if(str[i] == ' ' or str[i] == ',' or str[i] == '\t' or str[i] == '\n') {
                        if(word.size()) ret.push_back(word);
                        word = "";
                }
                else {
                        word += str[i];
                }
        }
        if(word.size()) ret.push_back(word);
        return ret;
}

inline void printError(string errorMsg) {
        cout << errorMsg << endl;
        exit(-1);
}

string complement_2s(string bin) {
        for(int i = 0; i < bin.size(); i++) {
                if(bin[i] == '0') bin[i] = '1';
                else bin[i] = '0';
        }
        int carry = 1;
        for(int i = (int)bin.size() - 1; i >= 0 and carry; i--) {
                if(bin[i] == '1') {
                        bin[i] = '0';
                }
                else {
                        bin[i] = '1';
                        carry = 0;
                }
        }
        return bin;
}

string toBinary(int x, int len = 4) {
        string ret;
        bool isNeg = (x<0);
        ret.resize(len, '0');
        for(int i = 0; i < len; i++) {
                if(x & 1) ret[i] = '1';
                x >>= 1;
        }
        reverse(ret.begin(), ret.end());
        return ret;
}

string toHex(int x, int len = 4) {
        if(len == 4) {
                stringstream stream;
                stream << hex << x;
                string result(stream.str());
                return result;
        }
        if(len == 8) {
                string result = "";
                string bin = toBinary(x, 8);
                string highBit = bin.substr(0, 4);
                string lowBit = bin.substr(4);

                x = 0;
                int p = 1;
                while(highBit.size()) {
                        x += p*(highBit.back()-'0');
                        p *= 2;
                        highBit.pop_back();
                }
                result += toHex(x, 4);

                x = 0;
                p = 1;
                while(lowBit.size()) {
                        x += p*(lowBit.back()-'0');
                        p *= 2;
                        lowBit.pop_back();
                }
                result += toHex(x, 4);

                return result;
        }
        return "?";
}

int main() {

        map<string, pii> instruction;
        ifstream input;
        string str;

        input.open("opcode.txt");
        input >> str;

        /**
         * instruction contains our group-specific opcodes for instructions, read from a file opcode.txt
         * Example:
         *      instruction["add"] = {9,     0}
         *                            ^      ^
         *                          opcode optype
        */
        for(int i = 0; i < str.size(); i++)
                instruction[ opList[str[i]-'A'].first ] = make_pair(i, opList[str[i]-'A'].second);

        input.close();
        input.open("input.txt");
        int lineCount = 0, instructionCount = 0;
        map<string, int> labelList;

        /**
         * Scanning the input file for all the labels
         * labelList tracks the line no in which each of the label appears
        */
        while(getline(input, str)) {
                if(str == "end") continue;
                if(str.back() == ':') {
                        str.pop_back();
                        string labelName = str;
                        labelList[str] = instructionCount;
                }
                else {
                        instructionCount++;
                }
        }
        input.close();

        /**
         * Reopening the input file. Now to convert MIPS to machine code
        */
        input.open("input.txt");
        lineCount = 0;
        instructionCount = 0;

        ofstream output;
        output.open("machine_code.txt");
        output << "v2.0 raw" << endl;

        while(getline(input, str)) {
        	lineCount++;

                if(str.empty()) continue;                       // read an empty line from file
                if(str == "end") return 0;                      // for debugging purpose. Stop file scanning when "end" is read
                if(str.back() == ':') continue;                 // found a label. Skip this line

                vector<string> words = getWords(str);
        	if(words.size() == 0) printError("Invalid operation on line " + to_string(lineCount));

                // R type instruction
                // Format: Opcode | Src_Reg_1 | Src_Reg_2 | Dst_Reg | Shft_Amnt
                // Bits:      4         4            4          4         4
                if(instruction[words[0]].optype == 0) {
                        if(words.size() != 4) printError("Insufficient operands for instruction on line " + to_string(lineCount));

                        string operation = words[0];
                        string rd = words[1];                   // destination reg
                        string rs1 = words[2];                  // source reg 1
                        string rs2 = words[3];                  // source reg 2
                        int shft = 0;

                        // for sll/srl instruction, there will be no Src_Reg_2, it will be Shft_Amnt
                        if(operation == "sll" or operation == "srl") {
                                shft = stoi(rs2);
                                rs2 = "$zero";
                        }

                        output << toHex(instruction[operation].opcode);
                        output << toHex(registerList[rs1]);
                        output << toHex(registerList[rs2]);
                        output << toHex(registerList[rd]);
                        output << toHex(shft);
                        output << endl;
                }
                // I type instruction
                // Format: Opcode | Src_Reg | Dst_Reg | Address/Immediate
                // Bits:     4         4         4             8
                else if(instruction[words[0]].optype == 1 and words[0] != "sw" and words[0] != "lw") {
                        if(words.size() != 4) printError("Insufficient operands for instruction on line " + to_string(lineCount));

                        string operation = words[0];
                        string rd = words[1];
                        string rs = words[2];
                        int immediate = 0;
                        /**
                         * - For branching instructions, the third word in the I type instruction will be a label, which
                         *    indicates an address
                         * - For other instructions, the third word in the I type instruction will be an immediate/offset
                         *   which will be an integer value
                        */
                        if(operation == "beq" or operation == "bneq") {
                                // branching is PC relative addressing
                                // immediate can be -ve as we can branch-jump to previous instruction
                                immediate = labelList[words[3]] - instructionCount - 1;
                        }
                        else {
                                immediate = stoi(words[3]);
                        }

                        output << toHex(instruction[operation].opcode);
                        output << toHex(registerList[rs]);
                        output << toHex(registerList[rd]);
                        output << toHex(immediate, 8);
                        output << endl;
                }
                // sw, lw instruction, special I type
                // Format: Opcode | Src_Reg | Dst_Reg | Address/Immediate
                // Bits:     4         4         4             8
                else if(instruction[words[0]].optype == 1) {
                        if(words.size() != 3) printError("Insufficient operands for instruction on line " + to_string(lineCount));

                        string operation = words[0];
                        string r1 = words[1];
                        string r2 = words[2];
                        int immediate = 0;

                        if(r2.back() == ')') {
                                string temp = r2;
                                reverse(temp.begin(), temp.end());
                                temp = temp.substr(1, 3);
                                reverse(temp.begin(), temp.end());
                                while(r2.back() != '(') r2.pop_back();
                                r2.pop_back();
                                immediate = stoi(r2);
                                r2 = temp;
                        }

                        /**
                         * MIPS assembly code format 
                         * lw: dst <- src
                         * sw: src -> dst
                        */
                        string rs = r1;
                        string rd = r2;
                        if(operation == "lw") swap(rs, rd);

                        output << toHex(instruction[operation].opcode);
                        output << toHex(registerList[rs]);
                        output << toHex(registerList[rd]);
                        output << toHex(immediate, 8);
                        output << endl;
                }
                // J type instruction
                else {
                        if(words.size() != 2) printError("Insufficient operands for instruction on line " + to_string(lineCount));
                        string operation = words[0];
                        int jumpAddress = labelList[words[1]];

                        output << toHex(instruction[operation].opcode);
                        output << toHex(jumpAddress, 8);
                        output << "00";
                        output << endl;
                }

                instructionCount++;
        }

        output.close();

        return 0;
}
