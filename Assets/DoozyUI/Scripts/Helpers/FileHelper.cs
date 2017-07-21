// Copyright (c) 2015 - 2016 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace DoozyUI
{
    public static class FileHelper
    {
        public static void writeObjectToFile<T>(string filePath, T obj, Action<FileStream, T> serializedMethod)
        {
            CreateDirectoryIfDoesntExist(filePath);

            var stream = new FileStream(filePath, FileMode.Create);
            serializedMethod(stream, obj);
            stream.Close();
        }

        public static T readObjectFile<T>(string filename, Func<FileStream, T> deserializationMethod)
        {
            if (!fileExists(filename))
                Debug.Log("ERROR: Can't load " + filename + " - no file exists");

            var stream = new FileStream(filename, FileMode.Open);
            var data = deserializationMethod(stream);
            stream.Close();

            return data;
        }

        public static void deleteObjectFile(string filePath)
        {
            File.Delete(filePath);
        }

        #region Misc Options

        public static FileInfo[] GetAllTheFilesFromFolder(string directoryPath)
        {
#if (UNITY_WEBPLAYER == false)
            var directoryInfo = new DirectoryInfo(directoryPath);

            var fileInfo = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            return fileInfo;
#else
        return null;
#endif
        }

        #endregion

        #region Validation

        public static bool fileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void CreateDirectoryIfDoesntExist(string filePath)
        {
#if (UNITY_WEBPLAYER == false)
            var file = new FileInfo(filePath);
            file.Directory.Create();
#endif
        }

        #endregion

        #region Serialization/Deserialization Methods

        public static void SerializeXML<T>(FileStream stream, T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, obj);
        }

        public static T DeserializeXML<T>(FileStream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(stream);
        }

        #endregion

        #region Get Folder Path

        /// <summary>
        ///     Searches for the folderName in all the project's directories and returns the absolute path of the first one it
        ///     encounters
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetFolderPath(string folderName)
        {
            var folderPath = Directory.GetDirectories(Application.dataPath, folderName, SearchOption.AllDirectories);

            if (folderPath == null)
            {
                Debug.LogError("You searched for the [" + folderName +
                               "] folder, but there is no folder with that name in this project.");
                return "ERROR";
            }
            if (folderPath.Length > 1)
            {
                Debug.LogWarning("You searched for the [" + folderName +
                                 "] folder and there are at least 2 folders with the same name in this project. Returned the folder location for the first one, but it might not be the one you're looking for so please give the folder you are looking for an unique name in the project.");
                return folderPath[0];
            }
            return folderPath[0];
        }

        /// <summary>
        ///     Searches for the folderName in all the project's directories and returns the relative path of the first one it
        ///     encounters
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetRelativeFolderPath(string folderName)
        {
            var folderPath = GetFolderPath(folderName);

            folderPath = folderPath.Replace(Application.dataPath, "Assets");

            return folderPath;
        }

        #endregion
    }
}