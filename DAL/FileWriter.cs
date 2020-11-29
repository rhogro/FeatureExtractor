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
        public void WriteToFile(string datasetName, SortedDictionary<string, double> attributes, Dictionary<string, int> classes, int[,] sparseMatrix, List<string>[] dataClasses)
        {
            WriteAttribuites(attributes);
            WriteClasses(classes);
            WriteBody(sparseMatrix, dataClasses);
            File.WriteAllText(string.Format(@"{0}.ariff", datasetName), stringBuilder.ToString());
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

        private void WriteBody(int[,] sparseMatrix, List<string>[] dataClasses)
        {
            string line;
            string classes;
            stringBuilder.AppendLine("\n@data");
            for(int i = 0; i < sparseMatrix.GetLength(0); i++)
            {
                line = "";
                classes = "";
                for(int j = 0; j < sparseMatrix.GetLength(1); j++)
                {
                    line += string.Format("{0},", sparseMatrix[i, j]);
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
