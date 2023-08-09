﻿using System;
using MauiTodo.Models;
using Newtonsoft.Json;

namespace MauiTodo.Services
{
    public class DataProvider : IDataProvider
	{
		public DataProvider()
		{
		}

        public async Task<T> Get<T>(int id)
        {
            var raw = await read();
            var data = JsonConvert.DeserializeObject<Data>(raw, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if(typeof(T) == typeof(TodoList))
            {
                var result = data.AllTodoLists.TodoLists.Where((e) => e.Id == id).First();
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return JsonConvert.DeserializeObject<T>(raw, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public Task Put<T>(T data)
        {
            var raw = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return write(raw);
        }

        async Task<string> read()
        {
            try
            {
                string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data.json");
                using FileStream InputStream = System.IO.File.OpenRead(targetFile);
                using StreamReader reader = new StreamReader(InputStream);
                var result = await reader.ReadToEndAsync();
                reader.Close();
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        async Task write(string raw)
        {
            try
            {
                string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data.json");
                using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
                using StreamWriter streamWriter = new StreamWriter(outputStream);
                await streamWriter.WriteAsync(raw);
                streamWriter.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}

