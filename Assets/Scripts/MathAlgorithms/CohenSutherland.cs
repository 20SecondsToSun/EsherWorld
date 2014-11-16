public class CohenSutherland 
{
		////////////////////////////////////////////
	////////////////////////////////////////////
	////////////////////////////////////////////
	private static int LEFT = 1;  /* двоичное 0001 */
	private static int RIGHT = 2;  /* двоичное 0010 */
	private static int BOT =  4;  /* двоичное 0100 */
	private static int TOP =  8;  /* двоичное 1000 */
	
	/* точка */
	public class point
	{
	   float x ;
	   float y ;
	}
	 
	/* прямоугольник */
	public class rect
	{
	   float x_min ;
	   float y_min ;
	   float x_max ;
	   float y_max ;
	};
	
	/* вычисление кода точки
	   r : указатель на struct rect; p : указатель на struct point */
	public static int vcode(rect r, point p)
	{
		int result = 0;//(p.x<r.x_min?LEFT:0)+(p.x>r.x_max?RIGHT:0)+(p.y<r.y_min?BOT:0)+(p.y>r.y_max?TOP:0);
		return result; 
	}
		
	/* если отрезок ab не пересекает прямоугольник r, функция возвращает -1;
   если отрезок ab пересекает прямоугольник r, функция возвращает 0 и отсекает
   те части отрезка, которые находятся вне прямоугольника */
	public static int evaluate (rect r, point a,point b)
	{
	       /* point c;  одна из точек 
	 		int code_a;
	 		int code_b;
	 		int code;
	 		
	        code_a = vcode(r, a);
	        code_b = vcode(r, b);
	 
	        // пока одна из точек отрезка вне прямоугольника 
	        while (true)//code_a | code_b != 0) 
	        {
	                // если обе точки с одной стороны прямоугольника, то отрезок не пересекает прямоугольник 
	                if (code_a & code_b)
	                        return -1;
	 
	                // выбираем точку c с ненулевым кодом 
	                if (code_a)
	                {
	                        code = code_a;
	                        c = a;
	                } 
	                else
	                {
	                        code = code_b;
	                        c = b;
	                }
	 
	                // если c левее r, то передвигаем c на прямую x = r->x_min
	                //   если c правее r, то передвигаем c на прямую x = r->x_max 
	                if (code & LEFT)
	                {
	                        c.y += (a.y - b.y) * (r.x_min - c.x) / (a.x - b.x);
	                        c.x = r.x_min;
	                } 
	                else if (code & RIGHT)
	                {
	                        c.y += (a.y - b.y) * (r.x_max - c.x) / (a.x - b.x);
	                        c.x = r.x_max;
	                }// если c ниже r, то передвигаем c на прямую y = r->y_min
	                 //   если c выше r, то передвигаем c на прямую y = r->y_max 
	                else if (code & BOT) {
	                        c.x += (a.x - b.x) * (r.y_min - c.y) / (a.y - b.y);
	                        c.y = r.y_min;
	                } else if (code & TOP) {
	                        c.x += (a.x - b.x) * (r.y_max - c.y) / (a.y - b.y);
	                        c.y = r.y_max;
	                }
	 
	                // обновляем код 
	                if (code == code_a)
	                        code_a = vcode(r,a);
	                else
	                        code_b = vcode(r,b);
	        }
	 
	        /* оба кода равны 0, следовательно обе точки в прямоугольнике */
	        return 0;
	}
	
}
