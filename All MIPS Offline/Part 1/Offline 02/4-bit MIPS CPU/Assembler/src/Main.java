import java.util.*;

public class Main {
    
    static final String inputFile = "Instructions.asm";
    static final String outputFile = "MachineCodes.bin";
	
	private static void p( String s ) {
        System.out.println(s);
    }

    public static void main(String[] args) {

        String str = Loader.loadLinesOfCode();
        System.out.println(str);
	    assert str != null;
	    List<String> list = Loader.loadArray(str);
        List<String> finalList = new ArrayList<>();
	    for (String aList : list) {
		    String res = run2( aList );
		    finalList.add( res );
	    }
        System.out.println("\n\n---------------------------------*********------------------------------");
        printList(finalList);
        Loader.writeList(finalList);
    }

    static void run() {
        //String binaryStr = "0010000100100111";
        String binaryStr = "add s1, s2, s3";// = sc.next();
        int decimal = Integer.parseInt(binaryStr, 2);
        String hexStr = Integer.toString(decimal, 16);
        System.out.println(hexStr);
    }//i added this for padding

    private static String run2( String str ) {
//        str = "add s1, s2, s4";
//        str = "sub s1, s2, s3";
//        str = "addi s3, s5, 7";
//        str = "lw s1, 10(s2)";
//        str = "j 15";
//        str = "beq s1, s2, 14";
//        str = "sw s1, 10(s2)";
//        str = "add s1, s2, s4";
        p(str);

        String[] firstArr = str.split(",");
//        System.out.println("Len " + firstArr.length);
        for (int i = 0; i < firstArr.length; i++) {
            firstArr[i] = firstArr[i].trim();
        }
//        printArr(firstArr);
        if (firstArr.length == 1) {
            return Jump(str);
        } else if (firstArr.length == 3) {
            if (firstArr[0].contains("beq")) {
                return Beq(firstArr);
            } else {
                return RItype(firstArr);
            }
        } else if (firstArr.length == 2) {
            return LoadOrStore(firstArr);
        }
        return null;
    }

    private static String RItype( String[] arr ) {
        String op = arr[0];
        String[] opArr = op.split(" ");

        opArr[1] = opArr[1].trim();
        String operation = opArr[0];
        String reg1 = opArr[1];
        String reg2 = arr[1];
        String reg3 = arr[2];
        return printBinary(operation, reg1, reg2, reg3);
    }

    private static String printBinary( String op , String reg1 , String reg2 , String reg3 ) {
        //p(op + " BIN " + reg1 + " BIN " + reg2 + " BIN " + reg3);
        String opBin = "";
        int opCode = -1;
        if (op.contains("add")) {
            opCode = 10;
            // opBin = "1010";
        } else if (op.contains("sub")) {
            opCode = 5;
            // opBin = "0101";
        } else if (op.contains("and")) {
            opCode = 1;
            // opBin = "0001";
        } else if (op.contains("or")) {
            opCode = 3;
            // opBin = "0011";
        }
        if (!reg3.contains( "$" )) {
            //I_TYPE
//            System.out.println("THIS IS I TYPE");
            if (op.contains("addi")) {
                opCode = 9;
                // opBin = "1011";
            } else if (op.contains("subi")) {
                opCode = 6;
                // opBin = "0110";
            } else if (op.contains("andi")) {
                opCode = 2;
                // opBin = "0010";
            } else if (op.contains("ori")) {
                opCode = 4;
                // opBin = "0100";
            }
        }
        String strDecimal = Integer.toString(opCode, 2);
        opBin = getBinary4Bits(strDecimal);
        char val;
        char[] arr = new char[1];
        String bin1, bin2, bin3;
        int n;

        val = reg1.charAt(1);
        arr[0] = val;
        bin1 = new String(arr);

        n = Integer.parseInt(bin1);
        bin1 = Integer.toString(n, 2);

        bin1 = getBinary4Bits(bin1);

        val = reg2.charAt(1);
        arr[0] = val;
        bin2 = new String(arr);

        n = Integer.parseInt(bin2);
        bin2 = Integer.toString(n, 2);

        bin2 = getBinary4Bits(bin2);

        if (reg3.contains("$")) {
            val = reg3.charAt(1);
            arr[0] = val;
            bin3 = new String(arr);

        } else {
            bin3 = reg3;
        }

        n = Integer.parseInt(bin3);
        String offset = bin3;
        bin3 = Integer.toString(n, 2);

        bin3 = getBinary4Bits(bin3);
        String full="";
//        String full = opBin + bin1 + bin2 + bin3;
        if (op.contains("addi") || op.contains("ori") || op.contains("andi") || op.contains("subi")){
            if(offset.charAt( 0 ) == '-'){
                n = Integer.parseInt(offset);
                n = 16 + n;
                offset = Integer.toString(n, 2 );
                full = opBin + bin2 +  bin1 + offset;
            }
            else{
                full = opBin + bin2 +  bin1 + bin3;
            }
        }
        else
            full = opBin + bin2 + bin3 + bin1;

        int decimal = Integer.parseInt(full, 2);
        String hexStr = Integer.toString(decimal, 16);

        full = opBin + " " + bin2 + " " + bin3 + " " + bin1;
        System.out.println(full);
        System.out.println(hexStr);

        System.out.println("\n");
        return hexStr;
    }

    private static String LoadOrStore( String[] arr ) {
//        System.out.println("THIS IS LOAD OR STORE");

        for (int i = 0; i < arr.length; i++) {
            arr[i] = arr[i].trim();
        }
        int opCode = -1;
//        printArr(arr);

        if (arr[0].contains("sw")) {
//            Store(arr);
//            System.out.println("STORE WORD");
            opCode = 7;
        } else if (arr[0].contains("lw")) {
//            Load(arr);
//            System.out.println("LOAD WORD");
            opCode = 8;
        }

        String[] opAndFirstReg = arr[0].split(" ");

        String sw = opAndFirstReg[0];
        String firstReg = opAndFirstReg[1];

        List<Character> c = new ArrayList<>();
        c.add(arr[1].charAt(arr[1].length() - 2));

//        System.out.println("Dest Reg Number: " + firstReg.charAt(1));
//        System.out.println("BaseReg Number : " + c.get(0));
        StringBuilder offset = new StringBuilder();

//        for(int i=arr[1].length() - 5; i >= 0; i--){
        for (int i = 0; i <= arr[1].length() - 5; i++) {
            offset.append( arr[1].charAt( i ) );
        }
//
//        System.out.println("Offset : " + offset);

        int destReg = firstReg.charAt(1) - '0';
        String destRegString = getBinary(destReg);

        int baseReg = c.get(0) - '0';
        String baseRegString = getBinary(baseReg);

        offset = Optional.ofNullable( getBinary( Integer.parseInt( offset.toString() ) ) ).map( StringBuilder::new ).orElse( null );

        String opCodeString = getBinary(opCode);

//        System.out.println("opCode : " + opCodeString);
//        System.out.println("baseReg: " + baseRegString);
//        System.out.println("destReg: " + destRegString);
//        System.out.println("offset: " + offset);
        String full = opCodeString + baseRegString + destRegString + offset;
        String hex = getHex(full);

        full = getSpaced(full);
        System.out.println(full);
        System.out.println(hex);
        return hex;

    }

    private static String Jump(String instruction) {
//        System.out.println("Inside jump : \n" + instruction);
        int opCode = 12;
        String opString = getBinary(opCode);

        String[] arr = instruction.split(" ");
        String num = arr[1];
        System.out.println("INSIDE JUMP: offset is " + num);
        String offsetString = getBinary(num); 

        String full = opString + getBinary(0) + getBinary(0) + offsetString;

        String hex = getHex(full);

        full = getSpaced(full);
        System.out.println(full);
        System.out.println(hex);
        return hex;
    }

    private static String Beq(String[] arr) {
//        System.out.println("Inside beq: ");
//        
//        printArr(arr);

        String[] firstLine = arr[0].split(" ");
        String reg1 = firstLine[1].trim();
        int reg1Num = reg1.charAt(1) - '0';

        int reg2Num = arr[1].charAt(1) - '0';

        
        
        int offset = Integer.parseInt(arr[2]);

        if(arr[2].contains( "-" )){
            offset = 16 + offset;
        }
        
        int opcode = 11;

        String opCodeStr = getBinary(opcode);
        String reg1Str = getBinary(reg1Num);
        String reg2Str = getBinary(reg2Num);
        String offsetStr = getBinary(offset);

        String full = opCodeStr + reg1Str + reg2Str + offsetStr;
        String hex = getHex(full);

        full = getSpaced(full);

        System.out.println(full);
        System.out.println(hex);

        return hex;
    }

    static void printArr(String[] arr) {
        for (String s : arr) {
            p(s);
        }
    }

    private static String getBinary4Bits(String s) {
        if (s.length() == 0) {
//            System.out.print(s + ""); 
            return s;
        } else {
            int len = s.length();
            int numrem = 4 - len;
	        StringBuilder strBuilder = new StringBuilder();
	        for (int i = 0; i < numrem; i++) {
                strBuilder.append( "0" );
            }
	        String str = strBuilder.toString();
	        str += s;
//            System.out.print(str + "");
            return str;
        }

    }

    private static String getBinary( String bin3 ) {
        try {
            int n = Integer.parseInt(bin3);
            bin3 = Integer.toString(n, 2);
            bin3 = getBinary4Bits(bin3);
        }catch(Exception e){
            System.out.println("NUMBER e jhamela : " + bin3);
            e.printStackTrace();
        }
        return bin3;
    }

    private static String getBinary( int n ) {
        String bin3 = Integer.toString(n, 2);
        bin3 = getBinary4Bits(bin3);
        return bin3;
    }

    private static String getHex( String bin ) {
        int decimal = Integer.parseInt(bin, 2);
	    return Integer.toString(decimal, 16);
    }

    private static String getSpaced(String full) {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < full.length(); i++) {
            if ((i % 4 == 0) && (i > 1)) {
                str.append( " " );
            }
            str.append( full.charAt( i ) );
        }
        return str.toString();
    }

    private static void printList(List<String> finalList) {
        for (String s : finalList) {
            System.out.println(s);
        }
    }

}


/*
Integer.toString(n,8) // decimal to octal

Integer.toString(n,2) // decimal to binary

Integer.toString(n,16) //decimal to Hex
 */
