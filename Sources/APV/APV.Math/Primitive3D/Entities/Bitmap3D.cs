using System;
using System.Drawing;

namespace APV.Math.Primitive3D.Entities
{
    public class Bitmap3D
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int[][] Data;
        public readonly int[][] BufferW;
        public readonly int[][] BufferEdge1;
        public readonly int[][] BufferEdge2;
        public readonly int Count;

        private float _minW = int.MaxValue;
        private float _maxW = -int.MaxValue;

        public Bitmap3D(int width, int height)
        {
            Width = width;
            Height = height;
            Count = width*height;
            Data = new int[width][];
            BufferW = new int[width][];
            BufferEdge1 = new int[width][];
            BufferEdge2 = new int[width][];
            for (int i = 0; i < width; i++)
            {
                Data[i] = new int[height];
                BufferW[i] = new int[height];
                BufferEdge1[i] = new int[2];
                BufferEdge2[i] = new int[2];
            }
        }

        public void ClearBuffers()
        {
            int width = Width;
            int height = Height;
            int zero = -int.MaxValue / 2;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    BufferW[i][j] = zero; // 0;// int.MaxValue;
                }
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int color = 0)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int errorX = 0;
            int errorY = 0;
            int incX;
            int incY;

            if (dx > 0)
            {
                incX = 1;
            }
            else if (dx == 0)
            {
                incX = 0;
            }
            else
            {
                incX = -1;
                dx = -dx;
            }

            if (dy > 0)
            {
                incY = 1;
            }
            else if (dy == 0)
            {
                incY = 0;
            }
            else
            {
                incY = -1;
                dy = -dy;
            }

            int d = (dy > dx) ? dy : dx;

            int currentX = x1;
            int currentY = y1;

            Data[currentX][currentY] = color;

            while ((currentX != x2) || (currentY != y2))
            {
                errorX = errorX + dx;
                errorY = errorY + dy;
                if (errorX >= d)
                {
                    errorX = errorX - d;
                    currentX = currentX + incX;
                }
                if (errorY >= d)
                {
                    errorY = errorY - d;
                    currentY = currentY + incY;
                }

                Data[currentX][currentY] = color;
            }
        }

        public void DrawTriangleEdge2(int x1, int y1, int w1, int x2, int y2, int w2, int[][] buffer)
        {
            int dx = (x2 - x1 >= 0 ? 1 : -1);
            int dy = (y2 - y1 >= 0 ? 1 : -1);

            int lengthX = (x2 - x1);
            int lengthY = (y2 - y1);

            int length = (lengthX > lengthY) ? lengthX : lengthY;

            if (length == 0)
            {
                //SetPixel(hdc, x1, y1, 0);
            }

            if (lengthY <= lengthX)
            {
                // Начальные значения
                int x = x1;
                int y = y1;
                int d = -lengthX;

                // Основной цикл
                length++;
                while (length > 0)
                {
                    length--;
                    //SetPixel(hdc, x, y, 0);
                    x += dx;
                    d += 2 * lengthY;
                    if (d > 0)
                    {
                        d -= 2 * lengthX;
                        y += dy;
                    }
                }
            }
            else
            {
                // Начальные значения
                int x = x1;
                int y = y1;
                int d = -lengthY;

                // Основной цикл
                length++;
                while (length > 0)
                {
                    length--;
                    //SetPixel(hdc, x, y, 0);
                    y += dy;
                    d += 2 * lengthX;
                    if (d > 0)
                    {
                        d -= 2 * lengthY;
                        x += dx;
                    }
                }
            }
        }

        private static void DrawTriangleEdge(int x1, int y1, int w1, int x2, int y2, int w2, int[][] buffer)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int dw = w2 - w1;
            int errorX = 0;
            int errorY = 0;
            int errorW = 0;
            int incX;
            int incY;
            int incW;

            if (dx > 0)
            {
                incX = 1;
            }
            else if (dx == 0)
            {
                incX = 0;
            }
            else
            {
                incX = -1;
                dx = -dx;
            }

            if (dy > 0)
            {
                incY = 1;
            }
            else if (dy == 0)
            {
                incY = 0;
            }
            else
            {
                incY = -1;
                dy = -dy;
            }

            if (dw > 0)
            {
                incW = 1;
            }
            else if (dw == 0)
            {
                incW = 0;
            }
            else
            {
                incW = -1;
                dw = -dw;
            }

            int d;
            if ((dx >= dy) && (dx >= dw))
            {
                d = dx;
            }
            else if ((dy >= dx) && (dy >= dw))
            {
                d = dy;
            }
            else
            {
                d = dw;
            }

            int currentX = x1;
            int currentY = y1;
            int currentW = w1;

            int[] bufferY = buffer[currentY];
            bufferY[0] = currentX;
            bufferY[1] = currentW;

            while ((currentX != x2) || (currentY != y2) || (currentW != w2))
            {
                errorX = errorX + dx;
                errorY = errorY + dy;
                errorW = errorW + dw;

                if (errorX >= d)
                {
                    errorX = errorX - d;
                    currentX = currentX + incX;
                }
                if (errorW >= d)
                {
                    errorW = errorW - d;
                    currentW = currentW + incW;
                }
                if (errorY >= d)
                {
                    errorY = errorY - d;
                    currentY = currentY + incY;
                    bufferY = buffer[currentY];
                    bufferY[0] = currentX;
                    bufferY[1] = currentW;
                }
            }
        }

        public void DrawTriangle3(Triangle3D triangle)
        {
            var ax = (int)triangle.A.I1;
            var ay = (int)triangle.A.I2;
            var bx = (int)triangle.B.I1;
            var by = (int)triangle.B.I2;
            var cx = (int)triangle.C.I1;
            var cy = (int)triangle.C.I2;
            int color = triangle.Color;

            if (((ax == bx) && (ay == by)) || ((ax == cx) && (ay == cy)) || ((bx == cx) && (by == cy)) ||
                ((ax == bx) && (ax == cx)) || ((ay == by) && (ay == cy)))
            {
                //Line
                return;
            }

            var aw = (int)triangle.A.I4;
            var bw = (int)triangle.B.I4;
            var cw = (int)triangle.C.I4;
            //var aw = (int)triangle.A.I3;
            //var bw = (int)triangle.B.I3;
            //var cw = (int)triangle.C.I3;

            if (aw > _maxW)
            {
                _maxW = aw;
            }
            if (aw < _minW)
            {
                _minW = aw;
            }

            int tx, ty, tz;
            // сортировка по y
            if (ay > by)
            {
                tx = ax;
                ax = bx;
                bx = tx;
                ty = ay;
                ay = by;
                by = ty;
                tz = aw;
                aw = bw;
                bw = tz;
            }
            if (ay > cy)
            {
                tx = ax;
                ax = cx;
                cx = tx;
                ty = ay;
                ay = cy;
                cy = ty;
                tz = aw;
                aw = cw;
                cw = tz;
            }
            if (by > cy)
            {
                tx = bx;
                bx = cx;
                cx = tx;
                ty = by;
                by = cy;
                cy = ty;
                tz = bw;
                bw = cw;
                cw = tz;
            }

            //var zMatrix = new Matrix33
            //{
            //    I11 = ax, I12 = bx, I13 = cx,
            //    I21 = ay, I22 = by, I23 = cy,
            //    I31 = az, I32 = bz, I33 = cz,
            //};
            //float zMatrixDet;
            //zMatrix = Matrix33.GetInvertible(zMatrix, out zMatrixDet);
            //if (zMatrixDet > 0)
            //{
            //}
            //var zVector = new Vector3(az, bz, cz);
            //zVector = zMatrix * zVector;

            int[][] bufferDoubleEdge = BufferEdge1;
            int[][] bufferLongEdge = BufferEdge2;
            DrawTriangleEdge(ax, ay, aw, bx, by, bw, bufferDoubleEdge);
            DrawTriangleEdge(bx, by, bw, cx, cy, cw, bufferDoubleEdge);
            DrawTriangleEdge(ax, ay, aw, cx, cy, cw, bufferLongEdge);

            //string abEdgeY = "";
            //string bcEdgeY = "";
            //string acEdgeY = "";

            for (int y = ay; y <= cy; y++)
            {
                //if (y <= by)
                //{
                //    abEdgeY += bufferDoubleEdge[y][1] + "; ";
                //    acEdgeY += bufferLongEdge[y][1] + "; ";
                //}
                //if (y >= by)
                //{
                //    bcEdgeY += bufferDoubleEdge[y][1] + "; ";
                //    acEdgeY += bufferLongEdge[y][1] + "; ";
                //}

                int leftX = bufferDoubleEdge[y][0];
                int leftW = bufferDoubleEdge[y][1];
                int rightX = bufferLongEdge[y][0];
                int rightW = bufferLongEdge[y][1];
                if (leftX != rightX)
                {
                    int dx = (rightX - leftX);
                    int dw = (rightW - leftW);

                    int errorX = 0;
                    int errorW = 0;
                    int incX;
                    int incW;

                    if (dx > 0)
                    {
                        incX = 1;
                    }
                    else if (dx == 0)
                    {
                        incX = 0;
                    }
                    else
                    {
                        incX = -1;
                        dx = -dx;
                    }

                    if (dw > 0)
                    {
                        incW = 1;
                    }
                    else if (dw == 0)
                    {
                        incW = 0;
                    }
                    else
                    {
                        incW = -1;
                        dw = -dw;
                    }

                    int d = (dw > dx) ? dw : dx;

                    int currentX = leftX;
                    int currentW = leftW;

                    while ((currentX != rightX) || (currentW != rightW))
                    {
                        int bufferW = BufferW[currentX][y];
                        double dW = currentW - bufferW;
                        if (dW > 0)
                        {
                            BufferW[currentX][y] = currentW;
                            Data[currentX][y] = color;
                        }

                        errorX = errorX + dx;
                        errorW = errorW + dw;

                        if (errorX >= d)
                        {
                            errorX = errorX - d;
                            currentX = currentX + incX;
                        }
                        if (errorW >= d)
                        {
                            errorW = errorW - d;
                            currentW = currentW + incW;
                        }
                    }


                    //if (leftX > rightX)
                    //{
                    //    int tX = leftX;
                    //    leftX = rightX;
                    //    rightX = tX;
                    //    int tW = leftW;
                    //    leftW = rightW;
                    //    rightW = tW;
                    //}


                    //double dw = (rightW - leftW) / (double)dx;
                    //double w = leftW;
                    //for (int x = leftX; x <= rightX; x++)
                    //{
                    //    int bufferW = BufferW[x][y];
                    //    double dW = w - bufferW;
                    //    if (dW > 0)
                    //    {
                    //        BufferW[x][y] = (int)System.Math.Round(w);
                    //        Data[x][y] = color;
                    //    }
                    //    w += dw;
                    //}
                }
            }
        }

        public void DrawTriangle(int ax, int ay, int bx, int by, int cx, int cy, int color = 0)
        {
            int dx, dy;
            // сортировка по y
            if (ay > by)
            {
                dx = ax;
                dy = ay;
                ax = bx;
                ay = by;
                bx = dx;
                by = dy;
            }
            if (ay > cy)
            {
                dx = ax;
                dy = ay;
                ax = cx;
                ay = cy;
                cx = dx;
                cy = dy;
            }
            if (by > cy)
            {
                dx = bx;
                dy = by;
                bx = cx;
                by = cy;
                cx = dx;
                cy = dy;
            }

            int x1;
            int x2;
            int x3;

            if (ay == cy)
            {
                if (ax < cx)
                {
                    x1 = ax;
                    x2 = cx;
                }
                else
                {
                    x1 = cx;
                    x2 = ax;
                }

                for (int x = x1; x <= x2; x++)
                {
                    Data[x][ay] = color;
                }
                return;
            }

            int dcax = cx - ax;
            int dcay = cy - ay;
            int dcby = cy - by;
            int dbay = by - ay;
            int dcbx = cx - bx;
            int dbax = bx - ax;

            x1 = ax;

            bool dcayz = (dcay != 0);
            bool dcbyz = (dcby != 0);
            bool dbayz = (dbay != 0);

            for (int y = ay; y <= by; y++)
            {
                if (dcayz)
                {
                    x1 = ax + dcax * (y - ay) / dcay;
                }

                x2 = (dbayz)
                    ? ax + dbax * (y - ay) / dbay
                    : ax;

                if (x1 > x2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                }

                for (int x = x1; x <= x2; x++)
                {
                    Data[x][y] = color;
                }
            }

            for (int y = by; y <= cy; y++)
            {
                if (dcayz)
                {
                    x1 = ax + dcax * (y - ay) / dcay;
                }

                x2 = (dcbyz)
                    ? bx + dcbx * (y - by) / dcby
                    : bx;

                if (x1 > x2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                }

                for (int x = x1; x <= x2; x++)
                {
                    Data[x][y] = color;
                }
            }
        }

        public void DrawTriangle2(int ax, int ay, int bx, int by, int cx, int cy, int color = 0)
        {
            if (((ax == bx) && (ay == by)) || ((ax == cx) && (ay == cy)) || ((bx == cx) && (by == cy)) ||
                ((ax == bx) && (ax == cx)) || ((ay == by) && (ay == cy)))
            {
                //Line
                return;
            }

            //Sort:
            //Sort by y
            int tx;
            int ty;
            if (ay > by)
            {
                tx = ax;
                ty = ay;
                ax = bx;
                ay = by;
                bx = tx;
                by = ty;
            }
            if (ay > cy)
            {
                tx = ax;
                ty = ay;
                ax = cx;
                ay = cy;
                cx = tx;
                cy = ty;
            }
            if (by > cy)
            {
                tx = bx;
                ty = by;
                bx = cx;
                by = cy;
                cx = tx;
                cy = ty;
            }

            DrawLine(ax, ay, bx, by, color);
            DrawLine(bx, by, cx, cy, color);
            DrawLine(cx, cy, ax, ay, color);

            var abEdgeN = new Vector3D(-ay + by, ax - bx, 0);
            var bcEdgeN = new Vector3D(-by + cy, bx - cx, 0);
            var caEdgeN = new Vector3D(-cy + ay, cx - ax, 0);
            var acEdgeN = new Vector3D(-ay + cy, ax - cx, 0);
            var cbEdgeN = new Vector3D(-cy + by, cx - bx, 0);
            var baEdgeN = new Vector3D(-by + ay, bx - ax, 0);

            var ppA = new Vector3D(bx - ax, by - ay, 0);
            var acNab = acEdgeN.I1 * ppA.I1 + acEdgeN.I2 * ppA.I2;

            bool left = (acNab < 0);

            var abEdgeNLength = (float)System.Math.Sqrt(abEdgeN.I1 * abEdgeN.I1 + abEdgeN.I2 * abEdgeN.I2);
            var bcEdgeNLength = (float)System.Math.Sqrt(bcEdgeN.I1 * bcEdgeN.I1 + bcEdgeN.I2 * bcEdgeN.I2);
            var caEdgeNLength = (float)System.Math.Sqrt(caEdgeN.I1 * caEdgeN.I1 + caEdgeN.I2 * caEdgeN.I2);
            var acEdgeNLength = (float)System.Math.Sqrt(acEdgeN.I1 * acEdgeN.I1 + acEdgeN.I2 * acEdgeN.I2);
            var cbEdgeNLength = (float)System.Math.Sqrt(cbEdgeN.I1 * cbEdgeN.I1 + cbEdgeN.I2 * cbEdgeN.I2);
            var baEdgeNLength = (float)System.Math.Sqrt(baEdgeN.I1 * baEdgeN.I1 + baEdgeN.I2 * baEdgeN.I2);

            abEdgeN.I1 = (abEdgeN.I1 / abEdgeNLength);
            abEdgeN.I2 = (abEdgeN.I2 / abEdgeNLength);
            bcEdgeN.I1 = (bcEdgeN.I1 / bcEdgeNLength);
            bcEdgeN.I2 = (bcEdgeN.I2 / bcEdgeNLength);
            caEdgeN.I1 = (caEdgeN.I1 / caEdgeNLength);
            caEdgeN.I2 = (caEdgeN.I2 / caEdgeNLength);
            acEdgeN.I1 = (acEdgeN.I1 / acEdgeNLength);
            acEdgeN.I2 = (acEdgeN.I2 / acEdgeNLength);
            cbEdgeN.I1 = (cbEdgeN.I1 / cbEdgeNLength);
            cbEdgeN.I2 = (cbEdgeN.I2 / cbEdgeNLength);
            baEdgeN.I1 = (baEdgeN.I1 / baEdgeNLength);
            baEdgeN.I2 = (baEdgeN.I2 / baEdgeNLength);

            Func<int, int, bool> isTriangle = (x0, y0) =>
            {
                var pA = new Vector3D(x0 - ax, y0 - ay, 0);
                var pB = new Vector3D(x0 - bx, y0 - by, 0);
                var pC = new Vector3D(x0 - cx, y0 - cy, 0);

                bool triangle;
                if (left)
                {
                    float abEdge = abEdgeN.I1*pA.I1 + abEdgeN.I2*pA.I2;
                    float bcEdge = bcEdgeN.I1*pB.I1 + bcEdgeN.I2*pB.I2;
                    float caEdge = caEdgeN.I1*pC.I1 + caEdgeN.I2*pC.I2;

                    triangle = ((abEdge > -1) && (bcEdge > -1) && (caEdge > -1));
                    //triangle = ((abEdge >= 0) && (bcEdge >= 0) && (caEdge >= 0));
                }
                else
                {
                    float acEdge = acEdgeN.I1 * pA.I1 + acEdgeN.I2 * pA.I2;
                    float cbEdge = cbEdgeN.I1 * pC.I1 + cbEdgeN.I2 * pC.I2;
                    float baEdge = baEdgeN.I1 * pB.I1 + baEdgeN.I2 * pB.I2;

                    triangle = ((acEdge > -1) && (cbEdge > -1) && (baEdge > -1));
                    //triangle = ((acEdge >= 0) && (cbEdge >= 0) && (baEdge >= 0));
                }
                return triangle;
            };

            var mx = (int)((ax + bx + cx) / 3f);
            var my = (int)((ay + by + cy) / 3f);
            //bool a = isTriangle(ax, ay);
            //bool b = isTriangle(bx, by);
            //bool c = isTriangle(cx, cy);
            //bool m = isTriangle(mx, my);

            int x = ax;
            int y = ay;
            int lastX = x;
            int lastY = y;

                bool moveLeft = true;
                bool canGoRight = true;

                //int minX = ax;
                //if (bx < minX)
                //{
                //    minX = bx;
                //}
                //if (cx < minX)
                //{
                //    minX = cx;
                //}

                //int maxX = ax;
                //if (bx > maxX)
                //{
                //    maxX = bx;
                //}
                //if (cx > maxX)
                //{
                //    maxX = cx;
                //}

            while (true)
            {
                //var v1 = new Vector3D(bx - x, by - y, 0);
                //var v2 = new Vector3D(ax - x, ay - y, 0);
                //var v3 = new Vector3D(cx - x, cy - y, 0);

                //var baEdge = baEdgeN.I1 * v1.I1 + baEdgeN.I2 * v1.I2;
                //var acEdge = acEdgeN.I1 * v2.I1 + acEdgeN.I2 * v2.I2;
                //var cbEdge = cbEdgeN.I1 * v3.I1 + cbEdgeN.I2 * v3.I2;

                //int baEdge = (y - by)*(ax - bx) - (ay - by)*(x - bx);
                //int acEdge = (y - ay)*(cx - ax) - (cy - ay)*(x - ax);
                //int cbEdge = (y - cy)*(bx - cx) - (by - cy)*(x - cx);
                //int topLeftEdge = baEdge;
                //bool triangle = ((baEdge >= 0) && (cbEdge >= 0) && (acEdge >= 0));
                //bool triangle = ((baEdge >= -1) && (cbEdge >= -1) && (acEdge >= -1));
                bool triangle = isTriangle(x, y);

                if (triangle)
                {
                    canGoRight = true;
                    //if ((cbEdge != 0) && (acEdge != 0))
                    {
                        Data[x][y] = color;
                    }
                }

                if (moveLeft)
                {
                    //if ((x > minX) && ((!canGoRight) || (triangle)))
                    if ((!canGoRight) || (triangle))
                    {
                        x = x - 1;
                    }
                    else
                    {
                        //Move right
                        x = lastX + 1;
                        y = lastY;
                        moveLeft = false;
                    }
                }
                else
                {
                    //Move Right:
                    //if ((x < maxX) && (triangle))
                    if (triangle)
                    {
                        x = x + 1;
                        lastX = x;
                    }
                    else
                    {
                        lastY = lastY + 1;
                        //lastX = lastX + 1;
                        if (lastY > cy)
                        {
                            break;
                        }
                        x = lastX;
                        y = lastY;
                        moveLeft = true;
                        canGoRight = false;
                    }
                }
            }

            //int caEdge = (y - cy) * (ax - cx) - (ay - cy) * (x - cx);
            //int abEdge = (y - ay) * (bx - ax) - (by - ay) * (x - ax);
            //int bcEdge = (y - by) * (cx - bx) - (cy - by) * (x - bx);

            //Draw line midle point
            DrawLine(mx - 1, my, mx + 1, my, Color.White.ToArgb());
            DrawLine(mx, my - 1, mx, my + 1, Color.White.ToArgb());

            //Draw edge normals
            //Edge 1
            Vector3D edge1N = (left) ? abEdgeN : baEdgeN;
            int edge1P1X = (left) ? (ax + bx) / 2 : (bx + ax) / 2;
            int edge1P1Y = (left) ? (ay + by) / 2 : (by + ay) / 2;
            var edge1NLength = (float)System.Math.Sqrt(edge1N.I1 * edge1N.I1 + edge1N.I2 * edge1N.I2);
            var edge1Nx = (edge1NLength > 0) ? (int)(10.0f * edge1N.I1 / edge1NLength) : 0;
            var edge1Ny = (edge1NLength > 0) ? (int)(10.0f * edge1N.I2 / edge1NLength) : 0;
            int edge1P2X = edge1P1X + edge1Nx;
            int edge1P2Y = edge1P1Y + edge1Ny;
            DrawLine(edge1P1X, edge1P1Y - 1, edge1P1X, edge1P1Y + 1, color);
            DrawLine(edge1P1X, edge1P1Y, edge1P2X, edge1P2Y, color);
            //Edge 2
            Vector3D edge2N = (left) ? bcEdgeN : acEdgeN;
            int edge2P1X = (left) ? (bx + cx) / 2 : (ax + cx) / 2;
            int edge2P1Y = (left) ? (by + cy) / 2 : (ay + cy) / 2;
            var edge2Length = (float)System.Math.Sqrt(edge2N.I1 * edge2N.I1 + edge2N.I2 * edge2N.I2);
            var edge2Nx = (edge2Length > 0) ? (int)(10.0f * edge2N.I1 / edge2Length) : 0;
            var edge2Ny = (edge2Length > 0) ? (int)(10.0f * edge2N.I2 / edge2Length) : 0;
            int edge2P2X = edge2P1X + edge2Nx;
            int edge2P2Y = edge2P1Y + edge2Ny;
            DrawLine(edge2P1X, edge2P1Y - 1, edge2P1X, edge2P1Y + 1, color);
            DrawLine(edge2P1X, edge2P1Y, edge2P2X, edge2P2Y, color);
            //Edge 3
            Vector3D edge3N = (left) ? caEdgeN : cbEdgeN;
            int edge3P1X = (left) ? (cx + ax) / 2 : (cx + bx) / 2;
            int edge3P1Y = (left) ? (cy + ay) / 2 : (cy + by) / 2;
            var edge3Length = (float)System.Math.Sqrt(edge3N.I1 * edge3N.I1 + edge3N.I2 * edge3N.I2);
            var edge3Nx = (edge3Length > 0) ? (int)(10.0f * edge3N.I1 / edge3Length) : 0;
            var edge3Ny = (edge3Length > 0) ? (int)(10.0f * edge3N.I2 / edge3Length) : 0;
            int edge3P2X = edge3P1X + edge3Nx;
            int edge3P2Y = edge3P1Y + edge3Ny;
            DrawLine(edge3P1X, edge3P1Y - 1, edge3P1X, edge3P1Y + 1, color);
            DrawLine(edge3P1X, edge3P1Y, edge3P2X, edge3P2Y, color);
        }

        public void DrawTriangle(Triangle3D triangle)
        {
            //var ax = (int)triangle.A.I1;
            //var ay = (int)triangle.A.I2;
            //var az = (int)triangle.A.I3;
            //var bx = (int)triangle.B.I1;
            //var by = (int)triangle.B.I2;
            //var bz = (int)triangle.B.I3;
            //var cx = (int)triangle.C.I1;
            //var cy = (int)triangle.C.I2;
            //var cz = (int)triangle.C.I3;
            //DrawTriangle(ax, ay, bx, by, cx, cy, triangle.Color);
            //DrawTriangle2(ax, ay, bx, by, cx, cy, triangle.Color);

            DrawTriangle3(triangle);
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, int color)
        {
            int t;
            if (x1 > x2)
            {
                t = x1;
                x1 = x2;
                x2 = t;
            }
            if (y1 > y2)
            {
                t = y1;
                y1 = y2;
                y2 = t;
            }
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    Data[x][y] = color;
                }
            }
        }

        public void DrawLine(Line3D line)
        {
            var x1 = (int)line.A.I1;
            var y1 = (int)line.A.I2;
            var x2 = (int)line.B.I1;
            var y2 = (int)line.B.I2;
            DrawLine(x1, y1, x2, y2, line.Color);
        }

        public void Clear(int color = 0)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Data[i][j] = color;
                }
            }
        }
    }
}