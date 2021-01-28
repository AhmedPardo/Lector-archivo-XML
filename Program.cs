using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace EjercicioBeClevar
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            XmlDocument doc = new XmlDocument();
            var organizations = new List<Beclever>();
            var organizationsBad = new List<Beclever>();
            var env = new XmlNamespaceManager(doc.NameTable);
            var wd = new XmlNamespaceManager(doc.NameTable);
            //string docPath =
             // Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //doc.Load(docPath + "/data.xml");
            doc.Load("./data.xml");
            env.AddNamespace("env", "http://schemas.xmlsoap.org/soap/envelope/");
            wd.AddNamespace("wd", "urn:com.workday/bsvc");

            XmlNode nodeEnv = doc.DocumentElement.SelectSingleNode("//env:Envelope", env).FirstChild;
            XmlNode nodeBody = nodeEnv.SelectSingleNode("//wd:Get_Organizations_Response", wd);
            XmlNode nodeResultsData = nodeBody.SelectSingleNode("//wd:Response_Data", wd);

            foreach (XmlNode node in nodeResultsData.ChildNodes)
            {
                bool ok = true;
                var organization = new Beclever();
                var id = node["wd:Organization_Reference"].FirstChild.InnerText;
                organization.WID = id;
                var refId = node["wd:Organization_Data"]["wd:Reference_ID"].InnerText;
                organization.Reference_ID = refId;
                var name = node["wd:Organization_Data"]["wd:Name"].InnerText;
                organization.Name = name;
                var type = node["wd:Organization_Data"]["wd:Organization_Type_Reference"].FirstChild.InnerText;
                var subtype = node["wd:Organization_Data"]["wd:Organization_Subtype_Reference"].FirstChild.InnerText;
                organization.Organization_Subtpe_ID = subtype;
                var extId = node["wd:Organization_Data"]["wd:External_IDs_Data"].LastChild.InnerText;
                organization.External_ID = extId;
                var isActive = node["wd:Organization_Data"]["wd:Inactive"];
                ok = isActive == null ? false : true;
                organization.IsInactive = isActive != null ? isActive.InnerText.Equals("0") ? "True" : "False" : "No hay datos";
                var superiorOrg = node["wd:Organization_Data"]["wd:Hierarchy_Data"].SelectNodes("wd:Superior_Organization_Reference", wd);
                var subOrg = node["wd:Organization_Data"]["wd:Hierarchy_Data"].SelectNodes("wd:Subordinate_Organization_Reference", wd);
                var topLevel = node["wd:Organization_Data"]["wd:Hierarchy_Data"]["wd:Top-Level_Organization_Reference"];

                //Console.Write("WID: {0}\n", id);
                //Console.Write("Reference ID: {0}\nName: {1}\n", refId, name);
                //Console.Write("Organization Type: {0}\nOrganization Subtype: {1}\n", type, subtype);
                //Console.Write("External ID: {0}\nIs Inactive: {1}\nTop-Level -> {2}\n", extId, isActive != null ? isActive.InnerText.Equals("0") ? "True" : "False" : "No existen datos", topLevel != null ? topLevel.LastChild.InnerText : "No hay datos");
                //Console.Write("Superior Organization:\n");

                if (superiorOrg.Count != 0)
                {
                    foreach (XmlNode subNodes in superiorOrg)
                    {
                        organization.Superior_Organization = subNodes.LastChild.InnerText;
                        //  Console.WriteLine("\t-{0}", subNodes.LastChild.InnerText);
                    }
                }
                else
                {
                    ok = false;
                    // Console.Write("\tNo hay datos\n");
                }

                // Console.Write("Subordinate Organization:\n");
                if (subOrg.Count != 0)
                {
                    foreach (XmlNode subNodes in subOrg)
                    {
                        // Console.WriteLine("\t-{0}", subNodes.LastChild.InnerText);
                    }
                }
                else
                {
                    ok = false;
                    //Console.Write("\tNo hay datos\n");
                }

                if (ok)
                {
                    organizations.Add(organization);

                }
                else
                {
                    organizationsBad.Add(organization);

                }


            }
            Console.WriteLine("\n\t\tORGANIZACIONES\n");

            foreach (Beclever org in organizations)
            {
                Console.WriteLine(org.ToString());
            }

            Console.WriteLine("\n\t\tORGANIZACIONES CON CARENCIA DE DATOS\n");

            foreach (Beclever org in organizationsBad)
            {
                Console.WriteLine(org.ToString());
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            CreateFile(organizations, true, nodeResultsData.ChildNodes.Count, elapsedMs);
            CreateFile(organizationsBad, false, nodeResultsData.ChildNodes.Count, elapsedMs);
        }

        public static void CreateFile(List<Beclever> list, bool ok, int total, long duration)
        {
            string docPath =
              Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = ok ? "Datos.txt" : "DatosIncompletos.txt";
            string pie = ok ? "\t\tTOTAL CORRECTOS: " : "\t\tTOTAL INCORRECTOS: ";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName)))
            {
                foreach (Beclever line in list)
                {
                    outputFile.WriteLine(line.ToString());
                }
                outputFile.WriteLine(pie + list.Count + "\tTOTAL ANALIZADOS: " + total +
                    "\tTIEMPO DE EJECUCIÓN APROXIMADO " + duration + " milisegundos.");
            }
        }

    }
}



