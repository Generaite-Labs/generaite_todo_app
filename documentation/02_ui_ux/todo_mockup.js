import React, { useState } from 'react';
import { PlusIcon, Search, SortAscIcon, CheckCircle2, Circle, Trash2, X } from 'lucide-react';

export default function InteractiveTodoAppMockup() {
  const [tasks, setTasks] = useState([
    { id: 1, title: "Complete project proposal", completed: true },
    { id: 2, title: "Review team's progress", completed: false },
    { id: 3, title: "Prepare for client meeting", completed: false },
  ]);
  const [newTask, setNewTask] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [sortAscending, setSortAscending] = useState(true);
  const [showAddTask, setShowAddTask] = useState(false);

  const addTask = () => {
    if (newTask.trim() !== "") {
      setTasks([...tasks, { id: Date.now(), title: newTask, completed: false }]);
      setNewTask("");
      setShowAddTask(false);
    }
  };

  const toggleTask = (id) => {
    setTasks(tasks.map(task =>
      task.id === id ? { ...task, completed: !task.completed } : task
    ));
  };

  const deleteTask = (id) => {
    setTasks(tasks.filter(task => task.id !== id));
  };

  const filteredTasks = tasks
    .filter(task => task.title.toLowerCase().includes(searchTerm.toLowerCase()))
    .sort((a, b) => {
      if (sortAscending) {
        return a.title.localeCompare(b.title);
      } else {
        return b.title.localeCompare(a.title);
      }
    });

  return (
    <div className="bg-gray-100 min-h-screen p-8">
      <div className="max-w-4xl mx-auto bg-white rounded-lg shadow-lg overflow-hidden">
        <div className="bg-blue-600 p-6">
          <h1 className="text-3xl font-bold text-white">My To-Do List</h1>
        </div>
        <div className="p-6">
          <div className="flex justify-between items-center mb-6">
            <button
              className="btn btn-ghost normal-case text-base font-medium text-blue-600 hover:bg-blue-50 transition-colors duration-300 flex items-center"
              onClick={() => setShowAddTask(true)}
            >
              <PlusIcon className="mr-2" size={20} />
              <span>Add Task</span>
            </button>
            <div className="flex items-center">
              <div className="relative">
                <input
                  type="text"
                  placeholder="Search tasks..."
                  className="input input-bordered pr-10 pl-10"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
              </div>
              <button
                className="btn btn-ghost ml-2"
                onClick={() => setSortAscending(!sortAscending)}
              >
                <SortAscIcon size={20} className={sortAscending ? "" : "transform rotate-180"} />
              </button>
            </div>
          </div>
          {showAddTask && (
            <div className="mb-4 p-4 bg-gray-50 rounded-lg flex items-center">
              <input
                type="text"
                placeholder="Enter new task..."
                className="input input-bordered flex-grow mr-2"
                value={newTask}
                onChange={(e) => setNewTask(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && addTask()}
              />
              <button
                className="p-2 rounded-full text-green-500 hover:bg-green-50 transition-colors duration-300"
                onClick={addTask}
              >
                <PlusIcon size={24} />
              </button>
              <button
                className="p-2 rounded-full text-gray-400 hover:bg-gray-100 transition-colors duration-300 ml-2"
                onClick={() => setShowAddTask(false)}
              >
                <X size={24} />
              </button>
            </div>
          )}
          <div className="space-y-4">
            {filteredTasks.map((task) => (
              <div key={task.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                <div className="flex items-center">
                  <button onClick={() => toggleTask(task.id)} className="mr-3">
                    {task.completed ? (
                      <CheckCircle2 className="text-green-500" size={24} />
                    ) : (
                      <Circle className="text-gray-300" size={24} />
                    )}
                  </button>
                  <span className={task.completed ? "line-through text-gray-500" : ""}>{task.title}</span>
                </div>
                <button onClick={() => deleteTask(task.id)}>
                  <Trash2 className="text-red-500 cursor-pointer" size={20} />
                </button>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}