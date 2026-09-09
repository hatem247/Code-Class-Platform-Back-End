# Code Class Platform — Comprehensive API Reference & Usage Guide

Welcome to the **Code Class Platform REST API Documentation**. This guide covers every available endpoint, authentication schemes, role permissions, request/response models, and executable usage examples.

---

## 📋 Table of Contents

- [Overview & Base Architecture](#overview--base-architecture)
- [Authentication & Authorization](#authentication--authorization)
- [Standard API Envelopes](#standard-api-envelopes)
- [Rate Limiting](#rate-limiting)
- [Seeded Demo Accounts](#seeded-demo-accounts)
- [API Endpoints](#api-endpoints)
  - [1. Authentication (`/api/auth`)](#1-authentication-apiauth)
    - [POST /api/auth/login](#post-apiauthlogin)
    - [POST /api/auth/logout](#post-apiauthlogout)
    - [POST /api/auth/refresh](#post-apiauthrefresh)
    - [GET /api/auth/me](#get-apiauthme)
    - [POST /api/auth/change-password](#post-apiauthchange-password)
  - [2. Account Management (`/api/accounts`)](#2-account-management-apiaccounts)
    - [GET /api/accounts](#get-apiaccounts)
    - [POST /api/accounts/students](#post-apiaccountsstudents)
    - [POST /api/accounts/teachers](#post-apiaccountsteachers)
    - [GET /api/accounts/{id}](#get-apiaccountsid)
  - [3. Administration & Analytics (`/api/admin`)](#3-administration--analytics-apiadmin)
    - [GET /api/admin/dashboard](#get-apiadmindashboard)
  - [4. Course Management (`/api/courses`)](#4-course-management-apicourses)
    - [GET /api/courses](#get-apicourses)
    - [GET /api/courses/{id}](#get-apicoursesid)
    - [POST /api/courses](#post-apicourses)
    - [PUT /api/courses/{id}](#put-apicoursesid)
    - [POST /api/courses/{id}/publish](#post-apicoursesidpublish)
    - [POST /api/courses/{id}/unpublish](#post-apicoursesidunpublish)
  - [5. Lecture Management (`/api/lectures`)](#5-lecture-management-apilectures)
    - [GET /api/lectures](#get-apilectures)
    - [GET /api/lectures/{id}](#get-apilecturesid)
    - [POST /api/lectures](#post-apilectures)
    - [PUT /api/lectures/{id}](#put-apilecturesid)
    - [DELETE /api/lectures/{id}](#delete-apilecturesid)
    - [POST /api/lectures/{id}/publish](#post-apilecturesidpublish)
    - [POST /api/lectures/{id}/unpublish](#post-apilecturesidunpublish)
  - [6. External Resources & Media (`/api/resources`)](#6-external-resources--media-apiresources)
    - [GET /api/resources/{id}](#get-apiresourcesid)
    - [POST /api/resources](#post-apiresources)
    - [PUT /api/resources/{id}](#put-apiresourcesid)
    - [DELETE /api/resources/{id}](#delete-apiresourcesid)
    - [POST /api/resources/{id}/verify](#post-apiresourcesidverify)
  - [7. Quizzes & Assessments (`/api/quizzes`)](#7-quizzes--assessments-apiquizzes)
    - [GET /api/quizzes](#get-apiquizzes)
    - [GET /api/quizzes/{id}](#get-apiquizzesid)
    - [POST /api/quizzes](#post-apiquizzes)
    - [POST /api/quizzes/{id}/questions](#post-apiquizzesidquestions)
    - [POST /api/quizzes/{id}/attempts/start](#post-apiquizzesidattemptsstart)
  - [8. Student Learning Progress (`/api/studentprogress`)](#8-student-learning-progress-apistudentprogress)
    - [GET /api/studentprogress/lecture-progress](#get-apistudentprogresslecture-progress)
    - [POST /api/studentprogress/lecture-progress](#post-apistudentprogresslecture-progress)

---

## 🌐 Overview & Base Architecture

- **Base URL (Local)**: `http://localhost:5039`
- **Protocol**: HTTP / HTTPS
- **Content Type**: `application/json`
- **Interactive Documentation**: `http://localhost:5039/swagger` (includes Dark Theme and Bearer Auth injector)
- **Database Engine**: Microsoft SQL Server (remote hosted / managed instance)

---

## 🔐 Authentication & Authorization

The API secures routes using **JSON Web Tokens (JWT Bearer Tokens)**.

1. Issue a `POST /api/auth/login` request with valid credentials.
2. Extract the returned `token` string from the JSON response.
3. Pass the token in all authenticated requests in the HTTP header:
   ```http
   Authorization: Bearer <your_jwt_token_here>
   ```

### User Roles & Policies

| Role | Access Scope |
| :--- | :--- |
| `ADMIN` | Complete administrative authority: manage all accounts, view metrics dashboard, publish/unpublish courses, manage lectures, quizzes, and resources. |
| `TEACHER` | Content creation & curation: create and update courses, lectures, quizzes, questions, and external resources. |
| `STUDENT` | Learning & assessment: view published catalog, track video progress, start quiz attempts. |
| `Anonymous` | Public read access to published courses, lectures, quizzes, and external resources. |

---

## 📦 Standard API Envelopes

### 1. Single Entity / Action Envelope (`ApiResponse<T>`)
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Operation completed successfully.",
  "data": { ... },
  "errors": null
}
```

### 2. Paginated Envelope (`PagedResult<T>`)
```json
{
  "items": [ ... ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5,
  "hasNext": true,
  "hasPrevious": false
}
```

### 3. Error Response Envelope
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Validation error description.",
  "data": null,
  "errors": [
    "Specific error description here."
  ]
}
```

---

## ⚡ Rate Limiting

The `/api/auth/login` endpoint is protected by a fixed-window rate limiter:
- **Max Requests**: 5 attempts
- **Window**: 15 minutes per client IP address
- **Exceeded Status**: `429 Too Many Requests`

---

## 👥 Seeded Demo Accounts

On first database initialization, three default accounts are automatically provisioned:

| Role | Email | Password |
| :--- | :--- | :--- |
| **Admin** | `admin@codeclass.local` | `Admin123!` |
| **Teacher** | `teacher@codeclass.local` | `Teacher123!` |
| **Student** | `student@codeclass.local` | `Student123!` |

---

## 🚀 API Endpoints

---

### 1. Authentication (`/api/auth`)

#### POST `/api/auth/login`
Authenticates a user via email and password, issuing a signed JWT Bearer token upon success.

- **Auth Required**: No (Public)
- **Rate Limit**: 5 requests / 15 minutes

**Request Body:**
```json
{
  "email": "admin@codeclass.local",
  "password": "Admin123!"
}
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Login successful.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "admin@codeclass.local",
    "accountId": 1,
    "role": "ADMIN"
  },
  "errors": null
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@codeclass.local","password":"Admin123!"}'
```

---

#### POST `/api/auth/logout`
Terminates the current user session.

- **Auth Required**: Yes (`Bearer <token>`)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Logout successful.",
  "data": null,
  "errors": null
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/auth/logout" \
  -H "Authorization: Bearer <TOKEN>"
```

---

#### POST `/api/auth/refresh`
Endpoint reserved for future token refresh workflow.

- **Auth Required**: No
- **Response**: `501 Not Implemented`

```json
{
  "success": false,
  "statusCode": 501,
  "message": "Refresh token flow is not implemented in this initial backend pass.",
  "data": null,
  "errors": [
    "Refresh token flow is not implemented in this initial backend pass."
  ]
}
```

---

#### GET `/api/auth/me`
Retrieves detailed account information and attached profile (Student or Teacher) for the currently authenticated caller.

- **Auth Required**: Yes (`Bearer <token>`)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Current user loaded.",
  "data": {
    "id": 2,
    "email": "teacher@codeclass.local",
    "role": "TEACHER",
    "isActive": true,
    "phoneNumber": "1111111111",
    "createdAt": "2026-09-09T12:07:44.819Z",
    "studentProfile": null,
    "teacherProfile": {
      "id": 1,
      "accountId": 2,
      "fullNameEn": "Default Teacher",
      "fullNameAr": "معلم افتراضي",
      "bioEn": "System teacher.",
      "bioAr": "معلم النظام.",
      "specializationEn": "Education",
      "specializationAr": "تعليم"
    }
  },
  "errors": null
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5039/api/auth/me" \
  -H "Authorization: Bearer <TOKEN>"
```

---

#### POST `/api/auth/change-password`
Changes the authenticated caller's account password.

- **Auth Required**: Yes (`Bearer <token>`)

**Request Body:**
```json
{
  "currentPassword": "Admin123!",
  "newPassword": "NewAdminPass123!#"
}
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Password changed successfully.",
  "data": null,
  "errors": null
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/auth/change-password" \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{"currentPassword":"Admin123!","newPassword":"NewAdminPass123!#"}'
```

---

### 2. Account Management (`/api/accounts`)

#### GET `/api/accounts`
Lists system user accounts with support for pagination, keyword search, role filtering, and active-status filtering.

- **Auth Required**: Yes (`ADMIN` role)
- **Query Parameters**:
  - `page` (integer, default: `1`)
  - `pageSize` (integer, default: `10`)
  - `search` (string, optional: matches email)
  - `role` (string, optional: `STUDENT`, `TEACHER`, `ADMIN`)
  - `isActive` (boolean, optional)

**Response (`200 OK`):**
```json
{
  "items": [
    {
      "id": 1,
      "email": "admin@codeclass.local",
      "role": "ADMIN",
      "isActive": true,
      "phoneNumber": "0000000000",
      "createdAt": "2026-09-09T12:07:44.819Z",
      "lastLoginAt": "2026-09-09T12:07:48.330Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 3,
  "totalPages": 1,
  "hasNext": false,
  "hasPrevious": false
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5039/api/accounts?page=1&pageSize=10&role=TEACHER" \
  -H "Authorization: Bearer <ADMIN_TOKEN>"
```

---

#### POST `/api/accounts/students`
Creates a new Student account and initializes the corresponding `StudentProfile`.

- **Auth Required**: Yes (`ADMIN` role)

**Request Body:**
```json
{
  "email": "ahmed.ali@example.com",
  "password": "StudentSecure123!",
  "phoneNumber": "+201012345678",
  "fullNameEn": "Ahmed Ali",
  "fullNameAr": "أحمد علي",
  "studentNumber": "STU-2026-001",
  "level": "Undergraduate",
  "grade": "Year 3",
  "major": "Software Engineering"
}
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Student created successfully.",
  "data": {
    "id": 4,
    "email": "ahmed.ali@example.com",
    "role": "STUDENT"
  },
  "errors": null
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/accounts/students" \
  -H "Authorization: Bearer <ADMIN_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "ahmed.ali@example.com",
    "password": "StudentSecure123!",
    "phoneNumber": "+201012345678",
    "fullNameEn": "Ahmed Ali",
    "fullNameAr": "أحمد علي",
    "studentNumber": "STU-2026-001",
    "major": "Software Engineering"
  }'
```

---

#### POST `/api/accounts/teachers`
Creates a new Teacher account and initializes the corresponding `TeacherProfile`.

- **Auth Required**: Yes (`ADMIN` role)

**Request Body:**
```json
{
  "email": "dr.hassan@example.com",
  "password": "TeacherSecure123!",
  "phoneNumber": "+201098765432",
  "fullNameEn": "Dr. Hassan Mahmoud",
  "fullNameAr": "د. حسن محمود",
  "specializationEn": "Cloud Computing & Distributed Systems",
  "specializationAr": "الحوسبة السحابية والأنظمة الموزعة",
  "qualificationEn": "Ph.D. in Computer Science",
  "qualificationAr": "دكتوراه في علوم الحاسب",
  "yearsOfExperience": 12
}
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Teacher created successfully.",
  "data": {
    "id": 5,
    "email": "dr.hassan@example.com",
    "role": "TEACHER"
  },
  "errors": null
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/accounts/teachers" \
  -H "Authorization: Bearer <ADMIN_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "dr.hassan@example.com",
    "password": "TeacherSecure123!",
    "fullNameEn": "Dr. Hassan Mahmoud",
    "specializationEn": "Cloud Computing",
    "yearsOfExperience": 12
  }'
```

---

#### GET `/api/accounts/{id}`
Loads account metadata by ID.

- **Auth Required**: Yes (`Bearer <token>`)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Account loaded.",
  "data": {
    "id": 2,
    "email": "teacher@codeclass.local",
    "role": "TEACHER",
    "isActive": true,
    "phoneNumber": "1111111111",
    "createdAt": "2026-09-09T12:07:44.819Z",
    "lastLoginAt": null
  },
  "errors": null
}
```

---

### 3. Administration & Analytics (`/api/admin`)

#### GET `/api/admin/dashboard`
Fetches real-time metric aggregates for the system dashboard (counts of students, teachers, active/inactive users, courses, lectures, quizzes, and recent account registrations).

- **Auth Required**: Yes (`ADMIN` role)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Dashboard metrics loaded.",
  "data": {
    "totalStudents": 150,
    "totalTeachers": 12,
    "activeUsers": 160,
    "inactiveUsers": 2,
    "totalCourses": 8,
    "totalLectures": 45,
    "totalQuizzes": 14,
    "recentAccounts": [
      {
        "id": 3,
        "email": "student@codeclass.local",
        "role": "STUDENT",
        "isActive": true,
        "createdAt": "2026-09-09T12:07:44.819Z"
      }
    ]
  },
  "errors": null
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5039/api/admin/dashboard" \
  -H "Authorization: Bearer <ADMIN_TOKEN>"
```

---

### 4. Course Management (`/api/courses`)

#### GET `/api/courses`
Lists published or all courses with pagination and text search across Arabic and English titles/descriptions.

- **Auth Required**: No (Public)
- **Query Parameters**:
  - `page` (integer, default: `1`)
  - `pageSize` (integer, default: `10`)
  - `search` (string, optional)
  - `published` (boolean, optional)

**Response (`200 OK`):**
```json
{
  "items": [
    {
      "id": 1,
      "nameEn": "Introduction to ASP.NET Core & C#",
      "nameAr": "مقدمة إلى ASP.NET Core ولغة سي شارب",
      "descriptionEn": "Master modern web development with .NET 10.",
      "descriptionAr": "احترف تطوير تطبيقات الويب الحديثة باستخدام دوت نت 10.",
      "isPublished": true,
      "isActive": true,
      "ordering": 1,
      "createdAt": "2026-09-09T12:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1,
  "hasNext": false,
  "hasPrevious": false
}
```

---

#### GET `/api/courses/{id}`
Retrieves detailed information for a specific course.

- **Auth Required**: No (Public)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Course loaded.",
  "data": {
    "id": 1,
    "nameEn": "Introduction to ASP.NET Core & C#",
    "nameAr": "مقدمة إلى ASP.NET Core ولغة سي شارب",
    "descriptionEn": "Master modern web development with .NET 10.",
    "descriptionAr": "احترف تطوير تطبيقات الويب الحديثة باستخدام دوت نت 10.",
    "isPublished": true,
    "isActive": true,
    "ordering": 1,
    "createdAt": "2026-09-09T12:00:00Z",
    "updatedAt": null
  },
  "errors": null
}
```

---

#### POST `/api/courses`
Creates a new course entity.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Request Body:**
```json
{
  "nameEn": "Data Structures & Algorithms",
  "nameAr": "تراكيب البيانات والخوارزميات",
  "descriptionEn": "Comprehensive DSA course in C#",
  "descriptionAr": "كورس شامل لتراكيب البيانات والخوارزميات",
  "isPublished": false,
  "ordering": 2
}
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Course created successfully.",
  "data": {
    "id": 2,
    "nameEn": "Data Structures & Algorithms",
    "nameAr": "تراكيب البيانات والخوارزميات",
    "descriptionEn": "Comprehensive DSA course in C#",
    "descriptionAr": "كورس شامل لتراكيب البيانات والخوارزميات",
    "isPublished": false,
    "ordering": 2,
    "isActive": true,
    "createdAt": "2026-09-09T12:15:00Z",
    "updatedAt": "2026-09-09T12:15:00Z"
  },
  "errors": null
}
```

---

#### PUT `/api/courses/{id}`
Updates details of an existing course.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Request Body:**
```json
{
  "nameEn": "Advanced Data Structures & Algorithms",
  "isPublished": true,
  "ordering": 1
}
```

---

#### POST `/api/courses/{id}/publish`
Publishes a course making it visible to students.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Course published.",
  "data": {
    "id": 2,
    "isPublished": true
  }
}
```

---

#### POST `/api/courses/{id}/unpublish`
Unpublishes a course from public visibility.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

---

### 5. Lecture Management (`/api/lectures`)

#### GET `/api/lectures`
Lists lectures with optional filtering by `courseId` and paginated navigation.

- **Auth Required**: No (Public)
- **Query Parameters**:
  - `page` (integer, default: `1`)
  - `pageSize` (integer, default: `10`)
  - `courseId` (integer, optional)

---

#### GET `/api/lectures/{id}`
Retrieves a single lecture by ID.

- **Auth Required**: No (Public)

---

#### POST `/api/lectures`
Creates a new lecture belonging to a course.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Request Body:**
```json
{
  "courseId": 1,
  "teacherId": 2,
  "titleEn": "Lesson 1: Introduction & Architecture",
  "titleAr": "الدرس 1: المقدمة والمعمارية",
  "descriptionEn": "Overview of ASP.NET Core and request lifecycle.",
  "descriptionAr": "نظرة عامة على دورة حياة الطلب في ASP.NET Core.",
  "durationSeconds": 1850,
  "order": 1,
  "isPublished": true,
  "isFree": true
}
```

---

#### PUT `/api/lectures/{id}`
Updates lecture metadata, order, duration, or publication status.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

---

#### DELETE `/api/lectures/{id}`
Permanently deletes a lecture entity.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

---

#### POST `/api/lectures/{id}/publish` and POST `/api/lectures/{id}/unpublish`
Toggles publication status for a lecture.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

---

### 6. External Resources & Media (`/api/resources`)

> 💡 **Google Drive Integration**: The platform does not store heavy video or PDF files on the API server. Teachers and admins register secure Google Drive links. The system automatically:
> 1. Validates the URL structure and host (`drive.google.com`, `docs.google.com`).
> 2. Extracts the unique Google Drive File ID.
> 3. Canonicalizes the URL to standard preview/view formats.
> 4. Classifies the media type (`File`, `Document`, `Presentation`, `Image`).

#### GET `/api/resources/{id}`
Retrieves registered external resource details.

- **Auth Required**: No (Public)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Resource loaded.",
  "data": {
    "id": 10,
    "provider": "GoogleDrive",
    "resourceType": "File",
    "originalUrl": "https://drive.google.com/file/d/1A2B3C4D5E6F7G8H9I0J/view?usp=sharing",
    "canonicalUrl": "https://drive.google.com/file/d/1A2B3C4D5E6F7G8H9I0J/view",
    "externalId": "1A2B3C4D5E6F7G8H9I0J",
    "fileName": "lecture-01-slides.pdf",
    "contentType": "application/pdf",
    "sizeBytes": null,
    "durationSeconds": null,
    "thumbnailUrl": null,
    "titleEn": "Lecture 01 Slides",
    "titleAr": "شرائح المحاضرة 1",
    "descriptionEn": "Official slide deck for Lecture 1",
    "descriptionAr": "العرض التقديمي الرسمي للمحاضرة الأولى",
    "isActive": true,
    "verificationStatus": "Verified",
    "lastVerifiedAt": "2026-09-09T12:20:00Z",
    "createdByAccountId": 2,
    "createdAt": "2026-09-09T12:18:00Z"
  },
  "errors": null
}
```

---

#### POST `/api/resources`
Registers a new Google Drive resource with automated validation.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Request Body:**
```json
{
  "url": "https://drive.google.com/file/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/view",
  "resourceType": "Document",
  "titleEn": "Course Syllabus",
  "titleAr": "مفردات المنهج",
  "descriptionEn": "Official syllabus in Google Docs",
  "descriptionAr": "الخطة الدراسية الرسمية",
  "fileName": "syllabus.pdf",
  "contentType": "application/pdf"
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/resources" \
  -H "Authorization: Bearer <TEACHER_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://drive.google.com/file/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/view",
    "titleEn": "Course Syllabus",
    "fileName": "syllabus.pdf"
  }'
```

---

#### PUT `/api/resources/{id}`
Updates details or the linked URL of an external resource.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

---

#### DELETE `/api/resources/{id}`
Removes a registered external resource.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

---

#### POST `/api/resources/{id}/verify`
Re-validates the registered resource URL against Google Drive URL formatting rules and updates `verificationStatus` to `"Verified"` or `"Failed"`.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Resource URL verified successfully.",
  "data": {
    "id": 10,
    "verificationStatus": "Verified",
    "lastVerifiedAt": "2026-09-09T12:22:15.123Z",
    "message": "Resource URL verified successfully."
  },
  "errors": null
}
```

---

### 7. Quizzes & Assessments (`/api/quizzes`)

#### GET `/api/quizzes`
Returns a paginated list of quizzes.

- **Auth Required**: No (Public)
- **Query Parameters**: `page` (default: 1), `pageSize` (default: 10)

---

#### GET `/api/quizzes/{id}`
Retrieves complete quiz structure, including nested questions and multiple-choice options.

- **Auth Required**: No (Public)

---

#### POST `/api/quizzes`
Creates a quiz attached to a course or lecture.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Request Body:**
```json
{
  "courseId": 1,
  "lectureId": 1,
  "teacherId": 2,
  "titleEn": "Quiz 1: C# Fundamentals",
  "titleAr": "اختبار 1: أساسيات لغة سي شارب",
  "descriptionEn": "Test your understanding of basic syntax and types.",
  "descriptionAr": "اختبر فهمك للأنظمة والأنواع الأساسية في سي شارب.",
  "timeLimitMinutes": 30,
  "passingScore": 70,
  "maxAttempts": 3,
  "isPublished": true
}
```

---

#### POST `/api/quizzes/{id}/questions`
Adds a question to an existing quiz.

- **Auth Required**: Yes (`ADMIN` or `TEACHER`)

**Request Body:**
```json
{
  "questionTextEn": "Which of the following is a value type in C#?",
  "questionTextAr": "أي مما يلي يعتبر Value Type في لغة سي شارب؟",
  "explanationEn": "int is a struct (value type), whereas string and object are reference types.",
  "explanationAr": "النوع int هو struct ويمثل قيمة، بينما string و object أنواع مرجعية.",
  "questionType": "MULTIPLE_CHOICE",
  "points": 5,
  "order": 1
}
```

---

#### POST `/api/quizzes/{id}/attempts/start`
Initializes a new quiz attempt for the calling student. Calculates attempt count and timestamps start time.

- **Auth Required**: Yes (`STUDENT` role)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Quiz attempt started.",
  "data": {
    "id": 1,
    "studentId": 3,
    "quizId": 1,
    "attemptNumber": 1,
    "startedAt": "2026-09-09T12:25:30.450Z",
    "submittedAt": null,
    "score": null,
    "isPassed": false
  },
  "errors": null
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/quizzes/1/attempts/start" \
  -H "Authorization: Bearer <STUDENT_TOKEN>"
```

---

### 8. Student Learning Progress (`/api/studentprogress`)

#### GET `/api/studentprogress/lecture-progress`
Fetches playback progress and completion metrics for all lectures taken by the authenticated student.

- **Auth Required**: Yes (`STUDENT` role)

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Progress loaded.",
  "data": [
    {
      "id": 1,
      "studentId": 3,
      "lectureId": 1,
      "watchedSeconds": 1420,
      "lastPositionSeconds": 1420,
      "completionPercentage": 75,
      "isCompleted": false,
      "firstOpenedAt": "2026-09-09T10:00:00Z",
      "lastWatchedAt": "2026-09-09T12:30:00Z"
    }
  ],
  "errors": null
}
```

---

#### POST `/api/studentprogress/lecture-progress`
Upserts a student's lecture progress (creates record if first view, or updates timestamp, watched duration, and completion percentage).

- **Auth Required**: Yes (`STUDENT` role)

**Request Body:**
```json
{
  "lectureId": 1,
  "watchedSeconds": 1850,
  "lastPositionSeconds": 1850,
  "completionPercentage": 100,
  "isCompleted": true
}
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Progress updated.",
  "data": {
    "id": 1,
    "studentId": 3,
    "lectureId": 1,
    "watchedSeconds": 1850,
    "lastPositionSeconds": 1850,
    "completionPercentage": 100,
    "isCompleted": true,
    "lastWatchedAt": "2026-09-09T12:35:10.123Z"
  },
  "errors": null
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5039/api/studentprogress/lecture-progress" \
  -H "Authorization: Bearer <STUDENT_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "lectureId": 1,
    "watchedSeconds": 1850,
    "lastPositionSeconds": 1850,
    "completionPercentage": 100,
    "isCompleted": true
  }'
```

---

## 🛠️ Status Code Conventions

| Code | Meaning | Context |
| :--- | :--- | :--- |
| `200 OK` | Success | Request succeeded and returned standard payload. |
| `400 Bad Request` | Validation Error | Missing required fields, invalid URL syntax, or schema violations. |
| `401 Unauthorized` | Unauthenticated | Missing or expired JWT Bearer token in the `Authorization` header. |
| `403 Forbidden` | Insufficient Role | Caller's role (`STUDENT`, `TEACHER`) lacks privilege for the requested action. |
| `404 Not Found` | Entity Missing | Target ID does not exist in the database. |
| `409 Conflict` | Unique Violation | Attempted to register an email that already exists. |
| `429 Too Many Requests` | Rate Limited | Exceeded rate limit thresholds on sensitive endpoints. |
| `501 Not Implemented` | Future Feature | Feature stubbed out for upcoming phase (e.g., refresh tokens). |

---

*Authored for Code Class Platform Back-End. Maintained with ASP.NET Core (.NET 10) & EF Core.*
