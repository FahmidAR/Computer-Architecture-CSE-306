import java.io.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Scanner;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Loader {

    public static String load() {
        try {
            Scanner in = new Scanner(new FileReader( Main.inputFile));

        } catch (FileNotFoundException ex) {
            ex.printStackTrace();
        }
    
        return "^";
    }

    private static String loadCode() throws Exception {
        File file = new File(Main.inputFile);

        BufferedReader br = new BufferedReader(new FileReader(file));

        String st;
        StringBuilder str = new StringBuilder();
        while ((st = br.readLine()) != null) {
//            System.out.println(st);
            str.append( st );
            str.append( "\n" );
        }
	
	    return str.toString();
//        return str.toString().replaceAll( "$sp","$7" );
    }

    static String loadLinesOfCode() {
        try {
            return loadCode();
            /*BufferedReader br = null;
            String everything = null;
            try {
            br = new BufferedReader(new FileReader("Mips_Assignment2_Input.txt"));
            } catch (FileNotFoundException ex) {
            System.out.println("FILE NOT FOUND!");
            ex.printStackTrace();
            }
            try {
            StringBuilder sb = new StringBuilder();
            String line = br.readLine();

            while (line != null) {
            sb.append(line);
            sb.append(System.lineSeparator());
            line = br.readLine();
            }
            everything = sb.toString();
            } catch (Exception e) {
            e.printStackTrace();
            } finally {
            try {
            br.close();
            } catch (IOException ex) {
            ex.printStackTrace();
            }
            }
            
            return everything;*/
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        return null ;
    }

    static List<String> loadArray(String str) {
        String[] arr = str.split("\n");
        List<String> list = new ArrayList<>();
        for (int i = 0; i < arr.length; i++) {
            if (Objects.equals( arr[i] , "\n" )) {
                //Ignore only lines
            } else {
                arr[i] = removeComments(arr[i]);
                list.add(arr[i]);
            }
        }

        return list;
    }

    private static String removeComments(String string) {
        String[] arr = string.split(";");
        if (arr.length == 1) {
            return string;
        } else {
            return arr[0];
        }
    }

    static void writeList(List<String> finalList) {
        PrintWriter writer = null;
        try {
            writer = new PrintWriter(Main.outputFile, "UTF-8");
            writer.println( "v2.0 raw" );
            for (String s : finalList) {
                writer.println(s);
            }
            writer.close();
        } catch (FileNotFoundException | UnsupportedEncodingException ex) {
            ex.printStackTrace();
        } finally {
            assert writer != null;
            writer.close();
        }
    }
}
