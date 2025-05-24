using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionDeTareas.Models
    {
        public class TaskQueue
        {
            private readonly Queue<Itarea> _cola = new();    
            private bool _procesando = false;               
            private readonly object _lock = new();    

            public void Enqueue(Itarea tarea)
            {
                lock (_lock)
                {

                    if (!_procesando)
                    {
                        _procesando = true;
                        _ = ProcesarColaAsync();
                    }
                }
            }
            private async Task ProcesarColaAsync()
            {
                while (true)
                {
                    Itarea tarea;

                    lock (_lock)
                    {
                        if (_cola.Count == 0)
                        {
                            _procesando = false;
                            return; 
                        }

                        tarea = _cola.Dequeue(); 
                    }

                    try
                    {
                        Console.WriteLine($"[TaskQueue] Procesando tarea: {tarea.Titulo}");
                        await tarea.EjecutarAsync();
                        Console.WriteLine($"[TaskQueue] Tarea completada: {tarea.Titulo}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[TaskQueue] Error al procesar tarea: {ex.Message}");
                    }
                }
            }
        }
    }
