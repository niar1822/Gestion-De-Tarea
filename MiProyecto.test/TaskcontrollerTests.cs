
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionDeTareas.Controllers;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaksModels.Models;
using Xunit;

public class TaskcontrollerTests
{
    private TaskContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new TaskContext(options);
        return context;
    }

    [Fact]
    public async Task Listartareas_ReturnsAllTasks()
    {
        var context = GetDbContext();
        context.Tasks.AddRange(
            new TaskData { Titulo = "Tarea 1", Description = "Desc 1" },
            new TaskData { Titulo = "Tarea 2", Description = "Desc 2" }
        );
        context.SaveChanges();

        var controller = new Taskcontroller(context, null, Mock.Of<ITaskNotificationService>());
        var result = await controller.Listartareas();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var tareas = Assert.IsAssignableFrom<IEnumerable<TaskData>>(okResult.Value);
        Assert.Equal(2, System.Linq.Enumerable.Count(tareas));
    }

    [Fact]
    public async Task VerTarea_Existe_ReturnsOk()
    {
        var context = GetDbContext();
        var tarea = new TaskData { Titulo = "Ver", Description = "Desc" };
        context.Tasks.Add(tarea);
        context.SaveChanges();

        var controller = new Taskcontroller(context, null, Mock.Of<ITaskNotificationService>());
        var result = await controller.VerTarea(tarea.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tarea.Id, ((TaskData)okResult.Value).Id);
    }

    [Fact]
    public async Task VerTarea_NoExiste_ReturnsNotFound()
    {
        var controller = new Taskcontroller(GetDbContext(), null, Mock.Of<ITaskNotificationService>());
        var result = await controller.VerTarea(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task EliminarTarea_Existe_ReturnsOk()
    {
        var context = GetDbContext();
        var tarea = new TaskData { Titulo = "Eliminar", Description = "Desc" };
        context.Tasks.Add(tarea);
        context.SaveChanges();

        var serviceMock = new Mock<ITaskNotificationService>();
        var controller = new Taskcontroller(context, null, serviceMock.Object);
        var result = await controller.EliminarTarea(tarea.Id);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task EliminarTarea_NoExiste_ReturnsNotFound()
    {
        var controller = new Taskcontroller(GetDbContext(), null, Mock.Of<ITaskNotificationService>());
        var result = await controller.EliminarTarea(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
