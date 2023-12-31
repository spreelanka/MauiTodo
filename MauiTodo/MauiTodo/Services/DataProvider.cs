﻿using System;
using System.Collections.ObjectModel;
using MauiTodo.Models;
using Newtonsoft.Json;

namespace MauiTodo.Services
{
    //placeholder for a more mature persistence strategy:
    // - hybrid local storage+rest api per business requirements
    //   - encrypted local storage
    //   - rest api
    public class DataProvider : IDataProvider
    {
        ReaderWriterLockSlim dataLock = new ReaderWriterLockSlim();
        Data data;
        TaskCompletionSource<bool> loaded = new TaskCompletionSource<bool>();
        public DataProvider()
        {
            Task.Run(async () =>
            {
                try
                {
                    var raw = await read();

                    data = JsonConvert.DeserializeObject<Data>(raw, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    loaded.SetResult(true);
                }
                catch (Exception ex)
                {

                }

            });
        }

        public Task Save()
        {
            return write(JsonConvert.SerializeObject(data));
        }

        public async Task<T> Get<T>(int id)
        {
            try
            {
                await loaded.Task;
                if (typeof(T) == typeof(TodoList))
                {
                    var result = data.AllTodoLists.TodoLists.Where(l => l.Id == id).FirstOrDefault();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                if (typeof(T) == typeof(Count))
                {
                    var result = data.Count;
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                if (typeof(T) == typeof(Data))
                {
                    return (T)Convert.ChangeType(data, typeof(T));
                }
                throw new NotSupportedException($"type {typeof(T).Name} not supported");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task Put<T>(T value)
        {
            try
            {
                await loaded.Task;
                if (typeof(T) == typeof(Data))
                {
                    data = (Data)Convert.ChangeType(value, typeof(Data));
                    return;
                }
                if (typeof(T) == typeof(TodoList))
                {
                    var list = (TodoList)Convert.ChangeType(value, typeof(TodoList));

                    if (data.AllTodoLists.TodoLists.Count == 0)
                    {
                        list.Id = 1;
                        data.AllTodoLists.TodoLists.Insert(0, list);
                        return;
                    }

                    var index = data.AllTodoLists.TodoLists.IndexOf(
                        data.AllTodoLists.TodoLists.Where(l => l.Id == list.Id).FirstOrDefault()
                        );
                    if (index != -1)
                    {
                        data.AllTodoLists.TodoLists[index] = list;
                    }
                    else
                    {
                        list.Id = data.AllTodoLists.TodoLists.Max(e => e.Id) + 1;
                        data.AllTodoLists.TodoLists.Insert(0, list);
                    }
                    return;
                }
                if (typeof(T) == typeof(Count))
                {
                    var count = (Count)Convert.ChangeType(value, typeof(Count));
                    data.Count = count;
                    return;
                }
                throw new NotSupportedException($"type {typeof(T).Name} not supported");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task<string> read()
        {
            //dataLock.TryEnterReadLock(50);
            try
            {
                string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data.json");
                using FileStream InputStream = System.IO.File.OpenRead(targetFile);
                using StreamReader reader = new StreamReader(InputStream);
                var result = await reader.ReadLineAsync();
                reader.Close();
                return result;
            }
            catch (FileNotFoundException ex)
            {
                await initDb();
                return await read();

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //if (dataLock.IsReadLockHeld)
                //    dataLock.ExitWriteLock();
            }
        }

        async Task write(string raw)
        {
            //dataLock.TryEnterWriteLock(100);
            try
            {
                string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data.json");
                using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
                using StreamWriter streamWriter = new StreamWriter(outputStream);
                await streamWriter.WriteLineAsync(raw);
                await streamWriter.FlushAsync();
                streamWriter.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //if (dataLock.IsWriteLockHeld)
                //    dataLock.ExitWriteLock();
            }
        }

        async Task initDb()
        {
            data = new Data
            {
                Count = new Count { Value = 0 },
                AllTodoLists = new AllTodoLists
                {
                    TodoLists = new ObservableCollection<TodoList>
                    {
                    }
                }
            };
            await Save();
        }
    }
}

