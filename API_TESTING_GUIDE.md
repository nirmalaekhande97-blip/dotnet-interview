# TODO API - Quick Testing Guide

## How to Run the Application

1. **Open Terminal in the TodoApi folder**
2. **Run the command:**
   ```bash
   dotnet run
   ```
3. **Look for the output that shows the URLs:**
   ```
   Now listening on: https://localhost:7186
   Now listening on: http://localhost:5186
   ```
4. **Access Swagger UI:**
   - Open browser: `https://localhost:7186/` or `http://localhost:5186/`

---

## Quick Test Sequence (Postman or Browser)

### **Step 1: Create a TODO**
```http
POST https://localhost:7186/api/todos
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk, bread, eggs",
  "isCompleted": false
}
```
**Response:** Returns the created TODO with ID 1

---

### **Step 2: Create Another TODO**
```http
POST https://localhost:7186/api/todos
Content-Type: application/json

{
  "title": "Finish homework",
  "description": "Math and English assignments",
  "isCompleted": false
}
```
**Response:** Returns the created TODO with ID 2

---

### **Step 3: Get All TODOs**
```http
GET https://localhost:7186/api/todos
```
**Response:** Returns array with both TODOs

---

### **Step 4: Get Specific TODO**
```http
GET https://localhost:7186/api/todos/1
```
**Response:** Returns the first TODO

---

### **Step 5: Update TODO**
```http
PUT https://localhost:7186/api/todos/1
Content-Type: application/json

{
  "title": "Buy groceries - DONE",
  "description": "Milk, bread, eggs - all purchased",
  "isCompleted": true
}
```
**Response:** Returns the updated TODO

---

### **Step 6: Delete TODO**
```http
DELETE https://localhost:7186/api/todos/2
```
**Response:** 204 No Content (success)

---

### **Step 7: Verify Deletion**
```http
GET https://localhost:7186/api/todos
```
**Response:** Returns array with only TODO #1

---

## Using Swagger UI (Recommended for Quick Testing)

1. **Run the application:** `dotnet run` in TodoApi folder
2. **Open browser:** Navigate to `https://localhost:7186/` 
3. **You'll see all endpoints listed**
4. **Click on any endpoint** (e.g., "POST /api/todos")
5. **Click "Try it out"**
6. **Enter your JSON data**
7. **Click "Execute"**
8. **See the response**

---

## Postman Import Instructions

1. **Open Postman**
2. **Click "Import"** (top left)
3. **Select "Upload Files"**
4. **Choose:** `TodoAPI.postman_collection.json` from the project root
5. **Click "Import"**
6. **Update the port number** if your app runs on a different port
7. **Start testing!**

---

## Testing Error Cases

### **Validation Error - Missing Title**
```http
POST https://localhost:7186/api/todos
Content-Type: application/json

{
  "title": "",
  "description": "This will fail"
}
```
**Expected Response:** 400 Bad Request with validation errors

---

### **Validation Error - Title Too Long**
```http
POST https://localhost:7186/api/todos
Content-Type: application/json

{
  "title": "This is a very long title that exceeds 200 characters... [repeat until > 200 chars]",
  "description": "Testing validation"
}
```
**Expected Response:** 400 Bad Request

---

### **Not Found Error**
```http
GET https://localhost:7186/api/todos/999
```
**Expected Response:** 404 Not Found
```json
{
  "message": "TODO item with ID 999 not found"
}
```

---

### **Update Non-Existent TODO**
```http
PUT https://localhost:7186/api/todos/999
Content-Type: application/json

{
  "title": "This won't work",
  "description": "TODO doesn't exist",
  "isCompleted": false
}
```
**Expected Response:** 404 Not Found

---

### **Delete Non-Existent TODO**
```http
DELETE https://localhost:7186/api/todos/999
```
**Expected Response:** 404 Not Found

---

## Sample cURL Commands (Alternative to Postman)

### **Create TODO**
```bash
curl -X POST https://localhost:7186/api/todos \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Test TODO\",\"description\":\"Testing with cURL\",\"isCompleted\":false}" \
  -k
```

### **Get All TODOs**
```bash
curl -X GET https://localhost:7186/api/todos -k
```

### **Get TODO by ID**
```bash
curl -X GET https://localhost:7186/api/todos/1 -k
```

### **Update TODO**
```bash
curl -X PUT https://localhost:7186/api/todos/1 \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Updated\",\"description\":\"Updated description\",\"isCompleted\":true}" \
  -k
```

### **Delete TODO**
```bash
curl -X DELETE https://localhost:7186/api/todos/1 -k
```

*Note: `-k` flag is used to ignore SSL certificate validation in development*

---

## HTTP Status Codes Reference

| Status Code | Meaning | When It Occurs |
|-------------|---------|----------------|
| 200 OK | Success | GET (found), PUT (updated) |
| 201 Created | Resource created | POST (successful creation) |
| 204 No Content | Success, no body | DELETE (successful) |
| 400 Bad Request | Validation error | Invalid input data |
| 404 Not Found | Resource not found | GET/PUT/DELETE non-existent ID |
| 500 Internal Server Error | Server error | Database or application error |

---

## Troubleshooting

### **Port Already in Use**
If you see "Address already in use", change the port in `Properties/launchSettings.json`

### **SSL Certificate Warning**
In development, you may see SSL warnings. You can:
- Trust the dev certificate: `dotnet dev-certs https --trust`
- Or use HTTP instead: `http://localhost:5186`

### **Database Issues**
- The database file `todos.db` is created automatically
- Located in: `TodoApi/bin/Debug/net8.0/todos.db`
- To reset: Stop the app, delete `todos.db`, restart

---

## Complete Test Workflow Example

1. **Start the API:** `dotnet run`
2. **Create 3 TODOs** (use different titles)
3. **Get all TODOs** ? Should return 3 items
4. **Get TODO #2** ? Should return specific item
5. **Update TODO #1** ? Mark as completed
6. **Delete TODO #3** ? Remove it
7. **Get all TODOs** ? Should return 2 items
8. **Try to get deleted TODO #3** ? Should return 404

---

**Happy Testing! ??**
