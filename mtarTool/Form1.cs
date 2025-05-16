using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;


namespace mtarTool
{
    public partial class Form1 : Form
    {
        private string mtarFilePath;
        private byte[] mtarFileData;
        private MtarHeader mtarHeader;
        private List<MtarData> dataTableEntries;

        public Form1()
        {
            InitializeComponent();
            btnBrowseHex.Enabled = false; // Disabled until a .mtar file is loaded
        }

        private void RunNoesis(string mtarPath)
        {
            string noesisPath = @"C:\Path\To\Noesis.exe";  // Update with actual path
            string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "human_model.mdn");

            if (!File.Exists(noesisPath) || !File.Exists(modelPath))
            {
                Log("Noesis tool or model file not found.");
                return;
            }

            // Set an environment variable for the MTAR file
            System.Environment.SetEnvironmentVariable("NOESIS_MGSMTAR_PATH", mtarPath);

            string arguments = $"\"{modelPath}\"";

            try
            {
                Process noesisProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = noesisPath,
                        Arguments = arguments,
                        UseShellExecute = false
                    }
                };

                noesisProcess.Start();
                Log("Noesis started with model.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start Noesis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log($"Failed to start Noesis: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Application started.");
        }

        /// <summary>
        /// Event handler for browsing and selecting a .mtar file.
        /// </summary>
        private void btnBrowseMtar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set the filter for .mtar files
                openFileDialog.Filter = "MTAR files (*.mtar)|*.mtar|All files (*.*)|*.*";
                openFileDialog.Title = "Select an .mtar File";

                // Show the file dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Display the file path in the TextBox
                    mtarFilePath = openFileDialog.FileName;
                    txtFilePath.Text = mtarFilePath;
                    Log($"Selected .mtar file: {mtarFilePath}");

                    // Load and parse the .mtar file
                    LoadMtarFile(mtarFilePath);
                }
            }
        }

        /// <summary>
        /// Loads and parses the selected .mtar file.
        /// </summary>
        /// <param name="filePath">Path to the .mtar file.</param>
        private void LoadMtarFile(string filePath)
        {
            try
            {
                mtarFileData = File.ReadAllBytes(filePath);
                if (mtarFileData.Length < 32)
                {
                    MessageBox.Show("The selected file is too small to be a valid .mtar file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log("Error: File is too small to contain a valid MtarHeader.");
                    return;
                }

                // Parse the MtarHeader
                byte[] headerBytes = mtarFileData.Take(32).ToArray();
                mtarHeader = MtarHeader.FromBytes(headerBytes);
                Log("Parsed MtarHeader:");
                Log(mtarHeader.ToString());

                // Validate magic number
                uint expectedMagic1 = 0x4D746172; // 'Mtar'
                uint expectedMagic2 = 0x7261744D; // 'ratM'
                if (mtarHeader.Magic != expectedMagic1 && mtarHeader.Magic != expectedMagic2)
                {
                    Log($"Warning: Unexpected magic number {mtarHeader.Magic:X8}. The file may be corrupted or in an unexpected format.");
                }
                else
                {
                    Log($"Magic number is valid: {mtarHeader.Magic:X8}");
                }

                // Parse DataTable
                ParseDataTable();

                // Extract anim_#.hex files
                ExtractAnimations();
                RunNoesis(mtarFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading .mtar file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log($"Error loading .mtar file: {ex.Message}");
            }
        }

        /// <summary>
        /// Parses the DataTable entries from the .mtar file.
        /// </summary>
        private void ParseDataTable()
        {
            try
            {
                dataTableEntries = new List<MtarData>();
                int numEntries = mtarHeader.NumMotion;
                int dataTableOffset = (int)mtarHeader.DataTableOffset;
                int entrySize = 16; // Each MtarData entry is 16 bytes

                for (int i = 0; i < numEntries; i++)
                {
                    int entryStart = dataTableOffset + i * entrySize;
                    if (entryStart + entrySize > mtarFileData.Length)
                    {
                        Log($"Error: DataTable entry {i} exceeds file size. Skipping.");
                        continue;
                    }

                    byte[] entryBytes = mtarFileData.Skip(entryStart).Take(entrySize).ToArray();
                    MtarData entry = MtarData.FromBytes(entryBytes);
                    dataTableEntries.Add(entry);
                }

                Log($"Parsed {dataTableEntries.Count} DataTable entries.");
                PopulateDataTableList();
                btnBrowseHex.Enabled = true; // Enable the Import Anim button
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing DataTable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log($"Error parsing DataTable: {ex.Message}");
            }
        }

        /// <summary>
        /// Populates the ListBox with DataTable entries.
        /// </summary>
        private void PopulateDataTableList()
        {
            listBoxEntries.Items.Clear();
            for (int i = 0; i < dataTableEntries.Count; i++)
            {
                var entry = dataTableEntries[i];
                string entryDisplay = $"Animation #{i}: Offset: 0x{entry.MtcmOffset:X4} |  Size: 0x{entry.MtcmSize:X4}";
                listBoxEntries.Items.Add(entryDisplay);
            }
            Log("DataTable entries loaded into the list.");
        }

        /// <summary>
        /// Event handler for importing a .hex animation file.
        /// </summary>
        private void btnBrowseHex_Click(object sender, EventArgs e)
        {
            if (listBoxEntries.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a DataTable entry to import the animation.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selectedEntryIndex = listBoxEntries.SelectedIndex;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set the filter for .hex files
                openFileDialog.Filter = "Hex Animation Files (*.hex)|*.hex|All files (*.*)|*.*";
                openFileDialog.Title = "Select a .hex Animation File";

                // Show the file dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string hexFilePath = openFileDialog.FileName;
                    Log($"Selected .hex animation file: {hexFilePath}");

                    // Proceed to import the animation
                    ImportAnimation(selectedEntryIndex, hexFilePath);
                }
            }
        }

        /// <summary>
        /// Imports a .hex animation file into the selected DataTable entry and saves the updated .mtar file.
        /// </summary>
        /// <param name="entryIndex">Index of the selected DataTable entry.</param>
        /// <param name="hexFilePath">Path to the .hex animation file.</param>
        private void ImportAnimation(int entryIndex, string hexFilePath)
        {
            try
            {
                // Read the .hex animation data
                byte[] animData = File.ReadAllBytes(hexFilePath);
                int animSize = animData.Length;
                Log($"Imported animation size: {animSize} bytes.");

                // Validate the selected entry
                if (entryIndex < 0 || entryIndex >= dataTableEntries.Count)
                {
                    MessageBox.Show("Selected entry index is out of range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log($"Error: Entry index {entryIndex} is out of range.");
                    return;
                }

                MtarData selectedEntry = dataTableEntries[entryIndex];

                // Calculate the maximum end offset across all DataTable entries
                long maxEnd = dataTableEntries.Max(entry => (long)(mtarHeader.MtcmOffset) + entry.MtcmOffset + entry.MtcmSize);
                Log($"Maximum end offset across all entries: 0x{maxEnd:X8}");

                // Set the insertion offset to the maximum end offset
                uint insertionOffset = (uint)maxEnd;
                Log($"Calculated insertion offset: 0x{insertionOffset:X8}");

                // Update the DataTable entry
                selectedEntry.MtcmOffset = insertionOffset - mtarHeader.MtcmOffset;
                selectedEntry.MtcmSize = (uint)animSize;
                Log($"Updated DataTable entry {entryIndex}: MtcmOffset=0x{selectedEntry.MtcmOffset:X8}, MtcmSize=0x{selectedEntry.MtcmSize:X8}");

                // Update the MtarHeader's MtexOffset to reflect the new end after insertion
                uint originalMtexOffset = mtarHeader.MtexOffset;
                mtarHeader.MtexOffset = insertionOffset + (uint)animSize;
                Log($"Updated MtarHeader MtexOffset from 0x{originalMtexOffset:X8} to 0x{mtarHeader.MtexOffset:X8}");

                // Reconstruct the updated file data
                List<byte> updatedFileData = mtarFileData.ToList();

                // Update the MtarHeader in the byte list
                byte[] updatedHeaderBytes = mtarHeader.ToBytes();
                for (int i = 0; i < updatedHeaderBytes.Length; i++)
                {
                    updatedFileData[i] = updatedHeaderBytes[i];
                }
                Log("MtarHeader updated in the file data.");

                // Update the DataTable entries in the byte list
                for (int i = 0; i < dataTableEntries.Count; i++)
                {
                    byte[] entryBytes = dataTableEntries[i].ToBytes();
                    int entryStart = (int)(mtarHeader.DataTableOffset + i * 16);
                    for (int j = 0; j < entryBytes.Length; j++)
                    {
                        if (entryStart + j < updatedFileData.Count)
                        {
                            updatedFileData[entryStart + j] = entryBytes[j];
                        }
                        else
                        {
                            Log($"Warning: Attempting to write beyond the file size at entry {i}, byte {j}. Skipping this byte.");
                        }
                    }
                }
                Log("DataTable entries updated in the file data.");

                // Insert the new animation data at the calculated insertion offset
                if (insertionOffset > updatedFileData.Count)
                {
                    MessageBox.Show("Insertion offset exceeds file size.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log("Error: Insertion offset exceeds file size.");
                    return;
                }

                updatedFileData.InsertRange((int)insertionOffset, animData);
                Log($"Inserted animation data at offset 0x{insertionOffset:X8}.");

                // Save the updated .mtar file with _edited suffix
                string newMtarPath = Path.Combine(Path.GetDirectoryName(mtarFilePath),
                                                 Path.GetFileNameWithoutExtension(mtarFilePath) + "_edited.mtar");
                File.WriteAllBytes(newMtarPath, updatedFileData.ToArray());
                Log($"Updated .mtar file saved as '{newMtarPath}'.");

                // Run Noesis for updated preview
                RunNoesis(newMtarPath);

                MessageBox.Show($"Animation imported successfully.\nSaved as '{newMtarPath}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing animation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log($"Error importing animation: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs messages to the status TextBox with a timestamp.
        /// </summary>
        /// <param name="message">Message to log.</param>
        private void Log(string message)
        {
            txtStatus.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}\r\n");
        }

        /// <summary>
        /// Extracts all anim_#.hex files from the .mtar file and saves them into the mtarout folder.
        /// </summary>
        private void ExtractAnimations()
        {
            try
            {
                // Define the output directory
                string outputDir = Path.Combine(Path.GetDirectoryName(mtarFilePath), "mtarout");

                // Create the directory if it doesn't exist; if it does, clear its contents
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true); // Delete and recreate to ensure clean state
                }
                Directory.CreateDirectory(outputDir);
                Log($"Created output directory: {outputDir}");

                // Iterate through each DataTable entry and extract anim_#.hex files
                for (int i = 0; i < dataTableEntries.Count; i++)
                {
                    var entry = dataTableEntries[i];
                    if (entry.MtcmSize == 0)
                    {
                        Log($"Entry {i}: MtcmSize is 0. Skipping extraction.");
                        continue;
                    }

                    // Calculate the absolute offset in the file
                    long animStart = mtarHeader.MtcmOffset + entry.MtcmOffset;
                    long animEnd = animStart + entry.MtcmSize;

                    if (animEnd > mtarFileData.Length)
                    {
                        Log($"Entry {i}: Animation data exceeds file size. Skipping extraction.");
                        continue;
                    }

                    // Extract the animation data
                    byte[] animData = mtarFileData.Skip((int)animStart).Take((int)entry.MtcmSize).ToArray();

                    // Define the output file path
                    string animFilePath = Path.Combine(outputDir, $"anim_{i}.hex");

                    // Write the animation data to the file
                    File.WriteAllBytes(animFilePath, animData);
                    Log($"Extracted Entry {i} to '{animFilePath}'.");
                }

                Log("Animation extraction completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting animations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log($"Error extracting animations: {ex.Message}");
            }
        }
    }
}