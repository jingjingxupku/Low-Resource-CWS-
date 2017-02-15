package wordtoVecInput;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;

public class postprocessing {
	private static String fullWidth2halfWidth(String fullWidthStr) {
        if (null == fullWidthStr || fullWidthStr.length() <= 0) {
            return "";
        }
        char[] charArray = fullWidthStr.toCharArray();
        //对全角字符转换的char数组遍历
        for (int i = 0; i < charArray.length; ++i) {
            int charIntValue = (int) charArray[i];
            //如果符合转换关系,将对应下标之间减掉偏移量65248;如果是空格的话,直接做转换
            if (charIntValue >= 65281 && charIntValue <= 65374) {
                charArray[i] = (char) (charIntValue - 65248);
            } else if (charIntValue == 12288) {
                charArray[i] = (char) 32;
            }
        }
        return new String(charArray);
    }
	public static void main(String[] args) throws IOException {
		// TODO Auto-generated method stub
		//answer 开头没有用
		postprocessing an=new postprocessing();
		String path1="answer.txt"; // change to the path which the test script outputs
		String path="msr_test.utf8";
		File file = new File(path);
		File file1 = new File(path1);
		BufferedReader reader = null;
		BufferedReader reader1 = null;
		BufferedWriter writer=null;
		reader = new BufferedReader(new FileReader(file));
		reader1 = new BufferedReader(new FileReader(file1));
		writer = new BufferedWriter(new FileWriter("myseg_test4new.txt")); // change to the output path  
		
		String str1="";
		while((str1=reader.readLine())!=null){
			//System.out.println(str1);
			String halfstr1=an.fullWidth2halfWidth(str1);
			String word="";int i=0;String writerstr1="";
			String str="";
			while((str=reader1.readLine())!=null){
				if(str.contains("EOS")){
					break;
				}
				if(str.contains("BOS")){
					continue;
				}
				String [] wordstr=str.split("\\[");
				word=wordstr[0];
				if(word.length()==0||word.equals("")){
					continue;
				}
				//System.out.println(str);
				//String label=reader1.readLine();
				String label=str.substring(str.indexOf("["), str.length());
				//while(label.length()==0||label.equals("")){
				//	label=reader1.readLine();
				//}
				String [] labels=label.split("\\] \\[");
				label=labels[0];
				if(label.contains("1")){
					label="b";	
				}
				else if(label.contains("0")){
					label="s";
				}
				else if(label.contains("2")){
					label="m";
				}
				else if(label.contains("3")){
					label="e";
				}
				//System.out.println(word+label);
				 /*if(label.contains("1, 0, 0, 0")){
					label="s";	
				}
				else if(label.contains("0, 1, 0, 0")){
					label="b";
				}*/
				/*else if(label.contains("0, 0, 0, 1")){
					label="e";
				}
				else if(label.contains("0, 0, 1, 0")){
					label="m";
				}*/
				if(word.contains("</s>")){
					continue;
				}
				if(!word.contains("d")&&!word.contains("e")){
					//System.out.println(word+label);
					//if(label.equals("b")&&writerstr1.length()>=1&&!writerstr1.endsWith((char)12288+"")){
			        //	writerstr1+=(char)12288+"";
					//}
					System.out.println(str1+" "+str1.length()+" "+i);
					writerstr1+=str1.charAt(i)+"";
					//if(word.contains("做")) System.out.println(label);
					if(label.equals("s")||label.equals("e")){
						writerstr1+=(char)12288+"";
					}
				}
				else if(word.contains("d")){
					String numbers="1234567890";String numword="";
					//if(label.equals("b")&&writerstr1.length()>=1&&!writerstr1.endsWith((char)12288+"")){
					//	writerstr1+=(char)12288+"";
					//}
						while(i<halfstr1.length()&&numbers.contains(halfstr1.charAt(i)+"")){
							//System.out.println(str1.charAt(i)+" "+i);
							numword+=(str1.charAt(i)+"");
							i++;
						}
						i--;
					writerstr1+=numword;
					if(label.equals("s")||label.equals("e")){
						writerstr1+=(char)12288+"";;
					}
				}
				else if(word.contains("e")){
					String numbers="abcdefghigklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
					String enumwrod="";
					//if(label.equals("b")&&writerstr1.length()>=1&&!writerstr1.endsWith((char)12288+"")){
					//	writerstr1+=(char)12288+"";
					//}
					while(i<halfstr1.length()&&numbers.contains(halfstr1.charAt(i)+"")){
						enumwrod+=(str1.charAt(i)+"");
						i++;
					}
					writerstr1+=enumwrod;
					if(label.equals("s")||label.equals("e")){
						writerstr1+=(char)12288+"";;
					}
					i--;
				}
				i++;
				
			}
		
			
			
			writer.write(writerstr1);
			writer.newLine();
		}
		writer.close();
		reader.close();
		reader1.close();

	}

}
