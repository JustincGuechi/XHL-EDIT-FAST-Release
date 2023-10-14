namespace XHL_Fast_Edit.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Fichiers XHL (*.xhl)|*.xhl|Dossiers ZIP (*.zip)|*.zip"
            };


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;

                textBox1.Text = selectedFilePath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (textBox1.Text != "")
            {
                if (textBox2.Text != "")
                {
                    string extension = Path.GetExtension(textBox1.Text).ToLower();
                    if (extension == ".xhl")
                    {
                        System.Xml.XmlDocument document = XhlEditEngine.Instance.XhlEdit(textBox1.Text, textBox2.Text);
                        string nomFichier = Path.GetFileName(textBox1.Text);
                        string nomFichierModifie = $"Traité {textBox2.Text} - {DateTime.Now.ToString("yyyyMMddHHmmss")} - {nomFichier}";
                        string repertoire = Path.GetDirectoryName(textBox1.Text);
                        string cheminFichierModifie = Path.Combine(repertoire, nomFichierModifie);
                        result = XhlEditEngine.Instance.XhlSave(cheminFichierModifie, document);
                    }
                    else if (extension == ".zip")
                    {
                        result = XhlEditEngine.Instance.ProcessZipFile(textBox1.Text, textBox2.Text);
                    }
                    if (result)
                    {
                        MessageBox.Show("L'opération a réussi !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("L'opération a échoué.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Merci de choisir un service.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Merci de choisir un dossier.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}