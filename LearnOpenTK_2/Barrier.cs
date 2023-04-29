using System.Collections.Generic;

namespace LearnOpenTK_2
{
    public class Barrier
    {
        // массив будет содержать в себе пары координат прямых: прямая от (1,2) до (3,4) будет записана как
        // coordPair = [[1,2,3,4],...]
        public List<double[]> coordPair = new List<double[]>();

        // в массив надо засунуть: [x, y, r]
        public List<double[]> circles = new List<double[]>();
    }
}
