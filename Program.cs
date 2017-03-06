using System;
using static System.Console;
using System.IO;
using System.Linq;

namespace Bme121.Pa3
{
    static partial class Program
    {
        static void Main( )
        {
            string inputFile  = @"input.csv";
            string outputFile = @"output.csv";
            int height;  // image height (number of rows)
            int width;  // image width (number of columns)
            Color[ , ] inImage;
            Color[ , ] outImage;
            
            // Read the input image from its csv file.
           FileStream inFile = new FileStream(inputFile, FileMode.Open, FileAccess.Read); 
		   StreamReader eye = new StreamReader(inFile);
		   
		   height = int.Parse(eye.ReadLine());
		   width = int.Parse(eye.ReadLine());
		   
		   inImage = new Color [height, width];
		   
		   for (int row =0; row<height; row++)
		   {
			   string[] pixelRow = eye.ReadLine().Split(','); 
			   
			   for (int col = 0; col<width; col++)
			   {
				   Color.FromArgb(
				   int.Parse(pixelRow[col*4]),
				   int.Parse(pixelRow[col*4+1]),
				   int.Parse(pixelRow[col*4+2]),
				   int.Parse(pixelRow[col*4+3]));
			   }
		   }
            
            // Generate the output image using Kirsch edge detection.
            
            outImage = new Color [height, width];
			
		   for (int row =0; row<height; row++)
		   {
			   for (int col = 0; col<width; col++)
			   {
					   if (row==0 || (row == height -1) || (col==0) || (col == width -1))
					   {
						   outImage[row,col]=inImage[row,col];
					   }
					   else
					   {
					
					   outImage[row,col]= GetKirschEdgeValue(inImage[row-1, col-1], inImage[row-1,col], inImage[row-1, col+1], 
														inImage[row,col-1],inImage[row, col],inImage[row, col+1],
														inImage[row+1, col-1],inImage[row+1, col],inImage[row+1, col+1]); 
				   
					   }				   
			   }
		   }
            // Write the output image to its csv file.
            FileStream outFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write); 
			StreamWriter newEye = new StreamWriter(outFile);
			
			newEye.WriteLine(height);
			newEye.WriteLine(width);	
					
		   for (int row =0; row<height; row++)
		   {
			   for (int col = 0; col<width; col++)
			   {
				   	newEye.Write("{0},{1},{2},{3},", outImage[row,col].A, outImage[row,col].R , outImage[row,col].G, outImage[row,col].B);
			   }
		   }
		   
			
			newEye.Dispose();
			outFile.Dispose();
		    eye.Dispose();
		    inFile.Dispose();
        }
        
        // This method computes the Kirsch edge-detection value for pixel color
        // at the centre location given the centre-location pixel color and the
        // colors of its eight neighbours.  These are numbered as follows.
        // The resulting color has the same alpha as the centre pixel, 
        // and Kirsch edge-detection intensities which are computed separately
        // for each of the red, green, and blue components using its eight neighbours.
        // c1 c2 c3
        // c4    c5
        // c6 c7 c8
        static Color GetKirschEdgeValue( 
            Color c1, Color c2,     Color c3, 
            Color c4, Color centre, Color c5, 
            Color c6, Color c7,     Color c8 )
        {
			  Color temp = new Color();
            int alpha = centre.A; 
            int red = GetKirschEdgeValue(c1.R,c2.R,c3.R,
										 c4.R,c5.R,c6.R, c7.R, c8.R);
			int green = GetKirschEdgeValue(c1.G,c2.G,c3.G,
										 c4.G,c5.G,c6.G, c7.G, c8.G);
			int blue = GetKirschEdgeValue(c1.B,c2.B,c3.B,
										 c4.B,c5.B,c6.B, c7.B, c8.B);								 							 
			temp = Color.FromArgb(alpha, red, green, blue); 	
            return temp;
        }
        
        // This method computes the Kirsch edge-detection value for pixel intensity
        // at the centre location given the pixel intensities of the eight neighbours.
        // These are numbered as follows.
        // i1 i2 i3
        // i4    i5
        // i6 i7 i8
        static int GetKirschEdgeValue( 
            int i1, int i2, int i3, 
            int i4,         int i5, 
            int i6, int i7, int i8 )
        {
			
		  int a1= (i1*5 + i2*5 + i3*5 + i4*-3 + i5*-3 + i6*-3 + i7*-3 + i8*-3);
		  int a2= (i1*-3 + i2*5 + i3*5 + i4*-3 + i5*5 + i6*-3 + i7*-3 + i8*-3);
		  int a3= (i1*-3 + i2*-3+ i3*5 + i4*-3 + i5*5 + i6*-3 + i7*-3 + i8*5);
		  int a4= (i1*-3 + i2*-3+ i3*-3 + i4*-3 + i5*5 + i6*-3 + i7*5 + i8*5);
		  int a5= (i1*-3 + i2*-3+ i3*-3 + i4*-3 + i5*-3 + i6*5 + i7*5 + i8*5);
		  int a6= (i1*-3 + i2*-3+ i3*-3 + i4*5 + i5*-3 + i6*5 + i7*5 + i8*-3);
		  int a7= (i1*5 + i2*-3+ i3*-3 + i4*5 + i5*-3 + i6*5 + i7*-3 + i8*-3);
		  int a8= (i1*5 + i2*5+ i3*-3 + i4*5 + i5*-3 + i6*-3 + i7*-3 + i8*-3);
		  
            return Math.Max(0, Math.Min(255, (Math.Max(a1, Math.Max(a2, Math.Max(a3 , Math.Max(a4,  Math.Max(a5, Math.Max(a6, Math.Max(a7, a8))))))))));
        }
    }
    
    // Implementation of part of System.Drawing.Color.
    // This is needed because .Net Core doesn't seem to include the assembly 
    // containing System.Drawing.Color even though docs.microsoft.com claims 
    // it is part of the .Net Core API.
    struct Color
    {
        int alpha;
        int red;
        int green;
        int blue;
        
        public int A { get { return alpha; } }
        public int R { get { return red;   } }
        public int G { get { return green; } }
        public int B { get { return blue;  } }
        
        public static Color FromArgb( int alpha, int red, int green, int blue )
        {
            Color result = new Color( );
            result.alpha = alpha;
            result.red   = red;
            result.green = green;
            result.blue  = blue;
            return result;
        }
    }
}
