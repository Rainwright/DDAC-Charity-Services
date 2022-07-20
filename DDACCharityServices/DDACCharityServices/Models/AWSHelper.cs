using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DDACCharityServices.Models
{
    public class AWSHelper
    {
        readonly public static string awsBucketName = "mvcflowershoptp050734";

        public static List<string> GetAWSCredentials()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfigurationRoot configure = builder.Build();

            List<string> keyLists = new List<string>();
            keyLists.Add(configure["AWSCredential:key1"]); // accesskey
            keyLists.Add(configure["AWSCredential:key2"]); // sessionkey
            keyLists.Add(configure["AWSCredential:key3"]); // tokenkey

            return keyLists;
        }

        public static AmazonS3Client GetAWSCredentialS3Object()
        {
            List<string> keyLists = GetAWSCredentials();
            return new AmazonS3Client(keyLists[0], keyLists[1], keyLists[2], RegionEndpoint.USEast1);
        }

        public static string ValidateImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length <= 0)
            {
                return "Image file is empty! Please upload a non-empty image!";
            }
            else if (imageFile.Length > 1048576) // 1MB
            {
                return "Image file exceeds 1MB in size! Images should be less than or equal to 1MB!";
            }
            else if (imageFile.ContentType.ToLower() != "image/png" && imageFile.ContentType.ToLower() != "image/jpeg" && imageFile.ContentType.ToLower() != "image/gif") // 1MB
            {
                return "Selected file is not a valid image file!";
            } else
            {
                return null;
            }
        }

        public static async Task<string> UploadImage(AmazonS3Client S3Client, IFormFile imageFile, string directory)
        {
            try
            {
                PutObjectRequest imageUploadRequest = new PutObjectRequest
                {
                    InputStream = imageFile.OpenReadStream(),
                    BucketName = awsBucketName + directory,
                    Key = imageFile.FileName,
                    CannedACL = S3CannedACL.PublicRead
                };

                await S3Client.PutObjectAsync(imageUploadRequest);

                return imageFile.FileName;
            } catch (AmazonS3Exception ex)
            {
                throw new Exception("S3 Error - " + ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected Error - " + ex);
            }
        }

        public static async Task DeleteImage(AmazonS3Client S3Client, string imageKey, string directory)
        {
            try
            {
                DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = awsBucketName + directory,
                    Key = imageKey,
                };

                await S3Client.DeleteObjectAsync(deleteObjectRequest);
            } catch (AmazonS3Exception ex)
            {
                throw new Exception("S3 Error - " + ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected Error - " + ex);
            }
        }

        public static string GetImageKey(string imageFileName, string s3DirectoryName)
        {
            return s3DirectoryName + "/" + imageFileName;
        }

        public static string GetFullImageUrl(string imageKey)
        {
            return "https://" + awsBucketName + ".s3.amazonaws.com" + imageKey;
        }
    }
}
