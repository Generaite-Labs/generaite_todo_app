```plantuml
    class User {
        +id: Int
        +username: String
        +email: String
        +password_hash: String
        +created_at: DateTime
        +updated_at: DateTime
    }
    class TodoItem {
        +id: Int
        +title: String
        +description: String
        +status: TaskStatus
        +due_date: Option~DateTime~
        +user_id: Int
        +task_list_id: Option~Int~
        +created_at: DateTime
        +updated_at: DateTime
    }
    class TodoItemList {
        +id: Int
        +name: String
        +user_id: Int
        +created_at: DateTime
        +updated_at: DateTime
    }
    class TaskStatus {
        <<enumeration>>
        TODO
        IN_PROGRESS
        DONE
    }
    User "1" -- "*" Task : has
    User "1" -- "*" TaskList : has
    TaskList "0..1" -- "*" Task : contains
```