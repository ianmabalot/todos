import axios from 'axios';
import React, { useState, useEffect } from 'react'

const Todos = () => {
    const [todos, setTodos] = useState([]);
    const [newTodo, setNewTodo] = useState([]);

    const apiBaseUrl = process.env.API_URL || "https://localhost:5001/api";

    console.log(apiBaseUrl);

    useEffect(() => {
        axios.get(`${apiBaseUrl}/Todos`)
            .then(response => setTodos(response.data))
            .catch(error => console.error(error));
    }, []);

    const addTodo = () => {
        const todo = { title: newTodo, isComplete: false };
        axios.post(`${apiBaseUrl}/todos`, todo)
            .then(response => setTodos([...todos, response.data]))
            .catch(error => console.error(error));
    };

    const toggleTodo = (id) => {
        const todo = todos.find(t => t.id === id);
        const updatedTodo = { ...todo, isComplete: !todo.isComplete };
        axios.put(`${apiBaseUrl}/todos/${id}`)
            .then(() => setTodos(todos.map(t => (t.id === id ? updatedTodo : t))))
            .catch(error => console.error(error));
    };

    const deleteTodo = (id) => {
        axios.delete(`${apiBaseUrl}/todos/${id}`)
            .then(() => setTodos(todos.filter(t => t.id !== id)))
            .catch(error => console.error(error));
    };

    return (
        <div>
            <h1>Todos</h1>
            <input
                type="text"
                value={newTodo}
                onChange={e => setNewTodo(e.target.value)}
            />
            <button onClick={addTodo}>Add Todo</button>
            <ul>
                {todos.map(todo => (
                    <li key={todo.id}>
                        <span style={{ textDecoration: todo.isComplete ? 'line-through' : 'none' }}>
                            {todo.title}
                        </span>
                        <button onClick={() => toggleTodo(todo.id)}>
                            {todo.isComplete ? 'Undo' : 'Complete'}
                        </button>
                        <button onClick={() => deleteTodo(todo.id)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default Todos;