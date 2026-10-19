namespace TicTacToe.Services
{
    /// <summary>
    /// Responsible for clearing user data, either upon request or upon account deletion.
    /// </summary>
    internal static class DataClearanceService
    {
        /// <summary>
        /// Deletes the user's data when the user's account gets deleted.
        /// </summary>
        /// <param name="rootDirectoryPath">The directory path containing the persistence files.</param>
        /// <param name="userId">The GUID of the user whose data is to be deleted.</param>
        /// <param name="isUserDeleted">Whether or not the user's account has been deleted.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the user's data directory cannot be found.</exception>
        /// <exception cref="IOException">Thrown if the data could not be deleted, for whatever reason.</exception>
        public static void DeleteUserData(string rootDirectoryPath, Guid userId, bool isUserDeleted = false)
        {
            string directoryPath = Path.Combine(rootDirectoryPath, userId.ToString());

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException(directoryPath);
            }

            try
            {
                if (isUserDeleted)
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
                else
                {
                    // Overwrite the files with empty data
                    var directoryFiles = Directory.GetFiles(directoryPath);
                    foreach (string filePath in directoryFiles)
                    {
                        // Implement minimal defensive guardrail to prevent exceptions from read-only files
                        if (File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly))
                        {
                            continue;
                        }

                        File.WriteAllText(filePath, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Error: could not delete user's data", ex);
            }
        }
    }
}