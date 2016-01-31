/*************************************************************************
 *     This file & class is part of the MIConvexHull Library Project. 
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     MIConvexHull is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     MIConvexHull is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with MIConvexHull.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on GraphSynth
 *     at http://miconvexhull.codeplex.com
 *************************************************************************/


using System;
using System.Linq;
using MIConvexHull;
using UnityEngine;

/// <summary>
/// A vertex is a simple class that stores the postion of a point, node or vertex.
/// </summary>
public class Cell : TriangulationCell<Vertex, Cell>
{

    double Det(double[,] m)
    {
        return m[0, 0] * ((m[1, 1] * m[2, 2]) - (m[2, 1] * m[1, 2])) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
    }

    double LengthSquared(double[] v)
    {
        double norm = 0;
        for (int i = 0; i < v.Length; i++)
        {
            var t = v[i];
            norm += t * t;
        }
        return norm;
    }

    Vector2 GetCircumcenter()
    {
        // From MathWorld: http://mathworld.wolfram.com/Circumcircle.html

        var points = Vertices;

        double[,] m = new double[3, 3];

        // x, y, 1
        for (int i = 0; i < 3; i++)
        {
            m[i, 0] = points[i].Position[0];
            m[i, 1] = points[i].Position[1];
            m[i, 2] = 1;
        }
        var a = Det(m);

        // size, y, 1
        for (int i = 0; i < 3; i++)
        {
            m[i, 0] = LengthSquared(points[i].Position);
        }
        var dx = -Det(m);

        // size, x, 1
        for (int i = 0; i < 3; i++)
        {
            m[i, 1] = points[i].Position[0];
        }
        var dy = Det(m);

        // size, x, y
        for (int i = 0; i < 3; i++)
        {
            m[i, 2] = points[i].Position[1];
        }
        //var c = -Det(m);

        var s = -1.0 / (2.0 * a);
        // I get a "not-used" warning on the below line. 
        //var r = Math.Abs(s) * Math.Sqrt(dx * dx + dy * dy - 4 * a * c);
        return new Vector2((float)(s * dx), (float)(s * dy)); 
    }

    Vector2 GetCentroid()
    {
        return new Vector2((float)Vertices.Select(v => v.Position[0]).Average(), (float)Vertices.Select(v => v.Position[1]).Average());
    }

    Vector2 circumCenter;
    bool _circumCenterCalculated = false;
    public Vector2 Circumcenter
    {
        get
        {
            if (!_circumCenterCalculated)
            {
                circumCenter = GetCircumcenter();
                _circumCenterCalculated = true;
            }
            return circumCenter;
        }
    }

    Vector2 centroid;
    bool _centroidCalculated = false;
    public Vector2 Centroid
    {
        get
        {
            if (!_centroidCalculated)
            {
                centroid = GetCentroid();
                _centroidCalculated = true;
            }
            return centroid;
        }
    }

    public Cell()
    {

    }
}