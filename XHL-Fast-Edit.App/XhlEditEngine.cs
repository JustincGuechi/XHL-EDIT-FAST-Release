using System.IO.Compression;
using System.Text;
using System.Xml;

namespace XHL_Fast_Edit.App
{
    public class XhlEditEngine
    {
        public static XhlEditEngine Instance => new XhlEditEngine();

        public XmlDocument XhlEdit(string cheminFichierXML, string service)
        {
            // Activer le support de l'encodage Windows-1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            XmlDocument documentXML = new XmlDocument();

            using (StreamReader sr = new StreamReader(cheminFichierXML, Encoding.GetEncoding("windows-1252")))
            {
                documentXML.LoadXml(sr.ReadToEnd());
            }

            string xpathExpression = $"//PayeIndivMensuel[not(Service[starts-with(@V, '{service}')])]";

            XmlNodeList elementsToDelete = documentXML.SelectNodes(xpathExpression);

            foreach (XmlNode element in elementsToDelete)
            {
                element.ParentNode?.RemoveChild(element);
            }

            return documentXML;

        }

        public bool XhlSave(string cheminFichierXML, XmlDocument documentXML)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.GetEncoding("windows-1252")
            };

            using (XmlWriter writer = XmlWriter.Create(cheminFichierXML, settings))
            {
                documentXML.Save(writer);
                return true;
            }
        }

        public bool ProcessZipFile(string cheminFichierZip, string service)
        {
            bool success = false;
            string zipFileNameWithoutExtension = Path.GetFileNameWithoutExtension(cheminFichierZip);

            string parentDirectory = Path.GetDirectoryName(cheminFichierZip);

            string extractPath = Path.Combine(parentDirectory, Guid.NewGuid().ToString());
            ZipFile.ExtractToDirectory(cheminFichierZip, extractPath);
            try
            {

                string[] xmlFiles = Directory.GetFiles(extractPath, "*.xhl");
                int totalXmlFiles = xmlFiles.Length;
                int increment = 1 / totalXmlFiles * 100;
                foreach (string xmlFile in xmlFiles)
                {
                    XmlDocument documentXML = XhlEdit(xmlFile, service);
                    success = XhlSave(xmlFile, documentXML);
                }

                string newZipFileName = $"Traité {service} - {DateTime.Now.ToString("yyyyMMddHHmmss")} - {zipFileNameWithoutExtension}.zip";
                string newZipPath = Path.Combine(parentDirectory, newZipFileName);

                ZipFile.CreateFromDirectory(extractPath, newZipPath);

                Directory.Delete(extractPath, true);
                return success;
            }
            catch (Exception ex)
            {
                Directory.Delete(extractPath, true);
                return false;
            }
        }
    }
}