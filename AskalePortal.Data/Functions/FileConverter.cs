using AskalePortal.Data.ResponseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.Functions
{
    public class FileConverter
    {

        static public ResponseByteArray convertByte(string destination, string file, string fileName)
        {
            ResponseByteArray response = new ResponseByteArray();
            byte[] fileBytes = System.IO.File.ReadAllBytes(destination + file);


            response.file = fileBytes;
            response.name = Path.Combine(destination, file);
            response.fileName = fileName;

            return response;
        }

    }

}
