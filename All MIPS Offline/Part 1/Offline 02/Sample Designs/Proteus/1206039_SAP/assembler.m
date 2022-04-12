clc;    
clear all;  
close all;
format compact;
single_instruction={'IN A','HLT','RET','CMP A,B','PUSH B','SHR A','OUT B'};
single_opcode=[0 1 2 3 5 8 11];
double_instruction={'AND A','MOV B'};
double_opcode=[7 13];
triple_instruction={'INC','XCHANGE','CALL','JAE','POP','DEC','MOV'};
triple_opcode=[4 6 9 10 12 14 15];

fp=fopen('Code.txt.','r');
fid=fopen('Code.bin','w');

dec=0;
tline = fgetl(fp);
k=0;
while ischar(tline)
 
    single=0;    
    double=0;    
    triple=0;          
    value=0; 
    for i=1:length(single_instruction)
        if  strcmpi(single_instruction(i),tline)
            dec=single_opcode(i)
            fwrite(fid,dec);       
            single=1;
            k=k+1;
            break;
        end       
    end
     
    if(single==0)
       for i=1:length(double_instruction)
           
           if    strncmpi(double_instruction(i),tline,length(char(double_instruction(i))))
                 dec=double_opcode(i)    
                 fwrite(fid,dec); 
                 k=k+1;
                 temp=length(char(double_instruction(i)));
                 byte=tline(temp+2:temp+3);      
                 dec=hex2dec(byte)
                 fwrite(fid,dec);        
                 double=1;
                 k=k+1;
                 break;
           end       
       end
    end

   if(single==0 && double==0)
        
        for i=1:length(triple_instruction)
                if  (strncmpi(triple_instruction(i),tline,length(char(triple_instruction(i)))))
                    dec=triple_opcode(i)     
                    fwrite(fid,dec);
                    k=k+1;
                    temp=length(char(triple_instruction(i)));  
                    gap=[2 4];       
                    byte=tline(temp+gap(2):temp+gap(2)+1);
                    dec=hex2dec(byte)
                    fwrite(fid,dec);    
                    k=k+1;
                    byte=tline(temp+gap(1):temp+gap(1)+1);
                    dec=hex2dec(byte)
                    fwrite(fid,dec);    
                    triple=1;  
                    k=k+1;
                    break;
                end
               
            end
   end
    
    if (single==0 && double==0 && triple==0 )
            if (length(tline)==4)
                byte=tline(1:4);
                byte=hex2dec(byte);
                    while k<(byte)
                        dec=0
                        fwrite(fid,dec);
                        k=k+1;
                    end
            elseif (length(tline)>=4)
                        byte=tline(1:4);
                        byte=hex2dec(byte);
                        
                            while k<(byte)
                                dec=0
                                fwrite(fid,dec);
                                k=k+1;
                            end
                        byte=tline(6:7);
                        byte=hex2dec(byte)
                        fwrite(fid,byte);
                        k=k+1;
            end
    
  value=1;
  end
        if(single==0 && double==0 && triple==0 && value==0)
            disp('Error in Code.....Aborting.......');
            break;
        end
    tline = fgetl(fp);
   
end

fclose(fp);
fclose(fid);