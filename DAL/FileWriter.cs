using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAL
{
    public class FileWriter
    {
        private readonly StringBuilder stringBuilder;

        public FileWriter()
        {
            stringBuilder = new StringBuilder();
        }
        public void WriteToFile(string datasetName, SortedDictionary<string, double> attributes, Dictionary<string, int> classes, int[,] matrix, List<string>[] dataClasses)
        {
            WriteAttribuites(attributes);
            WriteClasses(classes);
            WriteBody(matrix, dataClasses);
            File.WriteAllText(string.Format(@"{0}.arff", datasetName), stringBuilder.ToString());
        }

        private void WriteAttribuites(SortedDictionary<string, double> attributes)
        {
            foreach(var attribute in attributes)
            {
                stringBuilder.AppendLine(string.Format("@attribute {0} NUMERIC", attribute.Key));
            }
        }

        private void WriteClasses(Dictionary<string, int> classes)
        {
            string classesEnumeration = "";
            foreach (var documentClass in classes)
            {
                classesEnumeration += string.Format("{0},", documentClass.Key);
            }
            stringBuilder.AppendLine(string.Format("@attribute class {{{0}}}", classesEnumeration.TrimEnd(',')));
        }

        private void WriteBody(int[,] matrix, List<string>[] dataClasses)
        {
            string line;
            string classes;
            stringBuilder.AppendLine("\n@data");
            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                line = "";
                classes = "";
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    line += string.Format("{0},", matrix[i, j]);
                }

                foreach(string dataClass in dataClasses[i])
                {
                    classes += string.Format("{0},", dataClass);
                }
                stringBuilder.AppendLine(string.Format("{0} # {1}", line.TrimEnd(','), classes.TrimEnd(',')));
            }
        }
    }
}
