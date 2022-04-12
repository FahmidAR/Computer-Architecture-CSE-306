clc;
clear all;
close all;

A=xlsread('C:\Users\adnaan\Desktop\1206039_SAP\adn.xlsx','A:B');
fid1=fopen('C_ROM1.bin','w');
for i=1:length(A)
   value=A(i,1)*16+A(i,2); 
   fwrite(fid1,value);
end
fclose(fid1);


B=xlsread('C:\Users\adnaan\Desktop\1206039_SAP\adn.xlsx','C:D');
fid2=fopen('C_ROM2.bin','w');
for i=1:length(B)
   value=B(i,1)*16+B(i,2); 
   fwrite(fid2,value);  
end
fclose(fid2);

C=xlsread('C:\Users\adnaan\Desktop\1206039_SAP\adn.xlsx','E:F');
fid3=fopen('C_ROM3.bin','w');
for i=1:length(C)
   value=C(i,1)*16+C(i,2); 
   fwrite(fid3,value);  
end
fclose(fid3);


D=xlsread('C:\Users\adnaan\Desktop\1206039_SAP\adn.xlsx','G:G');
fid4=fopen('C_ROM4.bin','w');
for i=1:length(D)
   value=0*16+D(i,1); 
   fwrite(fid4,value);  
end
fclose(fid4);


E=xlsread('C:\Users\adnaan\Desktop\1206039_SAP\adn.xlsx','I:J');
fid5=fopen('ADD_ROM.bin','w');
for i=1:length(E)
   value=E(i,1)*16+E(i,2); 
   fwrite(fid5,value);  
end
fclose(fid5);