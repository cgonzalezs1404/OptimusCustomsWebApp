using Microsoft.AspNetCore.Components.Forms;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Data
{
    public static class Utils
    {
        public static FileModel StreamFile(Stream input)
        {
            FileModel result = new FileModel();
            try
            {
                var bytes = new byte[input.Length];
                input.ReadAsync(bytes, 0, (int)input.Length);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
