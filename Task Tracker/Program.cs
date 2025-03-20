using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class TaskItem
{
  public required int Id { get; set; }
  public string? description { get; set; }
  public required string status { get; set; }
  public required DateTime createdAt { get; set; } = DateTime.Now;
  public required DateTime updatedAt { get; set; } = DateTime.Now;
}

class Program
{
  static string filePath = "tasks.json";

  public static void Main(string[] args)
  {
    if (!File.Exists(filePath)) File.WriteAllText(filePath, "[]");

    if (args.Length == 0) 
    {
      Console.WriteLine("Usage: task-cli <command> [options]");
      return;
    }

    string command = args[0].ToLower();

    switch (command)
    {
      case "add":
        if (args.Length < 2) { Console.WriteLine("Usage: ..."); return; }
        AddTask(string.Join(" ", args.Skip(1)));
        break;
      case "update":
        if (args.Length < 3) { Console.WriteLine("Usage: ..."); return; }
        UpdateTask(int.Parse(args[1]), string.Join(" ", args.Skip(2)));
        break;
      case "delete":
        DeleteTask(int.Parse(args[1]));
        break;
      case "mark-in-progress":
        UpdateStatus(int.Parse(args[1]), "in-progress");
        break;
      case "mark-done":
        UpdateStatus(int.Parse(args[1]), "done");
        break;
      case "list":
        if (args.Length == 1) ListTask();
        else ListTask(args[1]);
        break;
      default:
        Console.WriteLine("Invalid command.");
        break;
    }
  }

  static List<TaskItem> LoadTasks()
  {
    return JsonConvert.DeserializeObject<List<TaskItem>>(File.ReadAllText(filePath)) ?? new List<TaskItem>();
  }

  static void SaveTasks(List<TaskItem> tasks)
  {
    File.WriteAllText(filePath, JsonConvert.SerializeObject(tasks, Formatting.Indented));
  }

  static void AddTask(string description)
  {
    var tasks = LoadTasks();
    var newTask = new TaskItem
    {
      Id = tasks.Count + 1,
      description = description,
      status = "In-progress",
      createdAt = DateTime.Now,
      updatedAt = DateTime.Now
    };

    tasks.Add(newTask);
    SaveTasks(tasks);
    Console.WriteLine($"Task added successfully (ID: {newTask.Id})");
  }

  static void UpdateTask(int id, string newDescription)
  {
    var tasks = LoadTasks();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task == null) { Console.WriteLine("Task not found."); return; }

    task.description = newDescription;
    task.updatedAt = DateTime.Now;
    SaveTasks(tasks);
    Console.WriteLine("Update task successfully");
  }

  static void DeleteTask(int id)
  {
    var tasks = LoadTasks();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task == null) { Console.WriteLine("Task not found."); return; }

    tasks.Remove(task);
    SaveTasks(tasks);
    Console.WriteLine("Task deleted successfully!"); 
  }

  static void UpdateStatus(int id, string newStatus)
  {
      var tasks = LoadTasks();
      var task = tasks.FirstOrDefault(t => t.Id == id);
      if (task == null) { Console.WriteLine("Task not found."); return; }

      task.status = newStatus;
      task.updatedAt = DateTime.Now;
      SaveTasks(tasks);
      Console.WriteLine($"Task {id} marked as {newStatus}.");
  }

  static void ListTask(string status = null) 
  {
    var tasks = LoadTasks();
    if (status != null)
    {
      tasks = tasks.Where(t => t.status == status).ToList();
      if (tasks.Count == 0) { Console.WriteLine($"No task found with status: {status}"); return; }
    }

    foreach (var task in tasks) 
    {
      Console.WriteLine($"[{task.Id}] {task.description} - {task.status} (Created: {task.createdAt}, Updated: {task.updatedAt})");
    }
  }
}

