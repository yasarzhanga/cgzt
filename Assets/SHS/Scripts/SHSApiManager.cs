using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// SHS API 数据管理器
/// 程序启动时自动从 Django REST API 预加载所有基础数据
/// 所有数据存储在静态字段中，其他脚本直接通过 SHSApiManager.字段 访问
///
/// API 地址：http://192.168.1.173:8012/back/
/// </summary>
public class SHSApiManager : MonoBehaviour
{
    // ==================== API 地址（修改这里适配不同服务器）====================
    private const string BASE_URL = "http://192.168.1.173:8012/back";

    private const string URL_UserSelect           = BASE_URL + "/UserSelect/";
    private const string URL_UserRegister         = BASE_URL + "/UserRegisterModel/";
    private const string URL_allProblemweb        = BASE_URL + "/allProblemweb/";
    private const string URL_School               = BASE_URL + "/School/";
    private const string URL_Classes              = BASE_URL + "/Classes/";
    private const string URL_Course               = BASE_URL + "/Course/";
    private const string URL_Chapter              = BASE_URL + "/Chapter/";
    private const string URL_Homework             = BASE_URL + "/Homework/";
    private const string URL_StudentManagement    = BASE_URL + "/StudentManagement/";
    private const string URL_KnowledgeGraph        = BASE_URL + "/KnowledgeGraph/";
    private const string URL_Workthumbnail        = BASE_URL + "/Workthumbnail/";

    // ==================== 单例 ====================
    private static SHSApiManager _instance;
    public static SHSApiManager Instance => _instance;

    // ==================== 生命周期 ====================
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        LoadAllData();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    // ==================== 加载控制 ====================
    private bool _isLoading = false;

    /// <summary>全部数据是否已加载完成</summary>
    public bool IsReady { get; private set; } = false;

    /// <summary>
    /// 程序启动时调用，一次性加载所有 API 数据
    /// </summary>
    public void LoadAllData()
    {
        if (_isLoading || IsReady) return;
        _isLoading = true;
        Debug.Log("[SHSApiManager] 开始预加载 API 数据...");
        StartCoroutine(LoadAllDataCoroutine());
    }

    private IEnumerator LoadAllDataCoroutine()
    {
        // 并行启动所有请求
        StartCoroutine(LoadUserList());
        StartCoroutine(LoadProblemList());
        StartCoroutine(LoadSchoolList());
        StartCoroutine(LoadClassList());
        StartCoroutine(LoadCourseList());
        StartCoroutine(LoadChapterList());
        StartCoroutine(LoadHomeworkList());
        StartCoroutine(LoadKnowledgeGraph());
        StartCoroutine(LoadWorkthumbnail());

        // 等待所有请求完成（以最长一个为准，最多15秒）
        float elapsed = 0f;
        while (elapsed < 15f)
        {
            if (_loadCount <= 0) break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        IsReady = true;
        _isLoading = false;
        OnAllDataLoaded?.Invoke();
        Debug.Log("[SHSApiManager] 全部 API 数据加载完成。");

        // 自动开始三轮压力测试
        Debug.Log("========== 压力测试开始 ==========");
        StartCoroutine(AutoStressTest());
    }

    private IEnumerator AutoStressTest()
    {
        yield return new WaitForSeconds(1f);

        // 第一轮：GET UserSelect 10次
        Debug.Log("========== 第1轮: GET UserSelect 10次 ==========");
        yield return StartCoroutine(StressTest_GETUserSelectCoroutine(10));

        yield return new WaitForSeconds(1f);

        // 第二轮：GET allProblemweb 10次
        Debug.Log("========== 第2轮: GET allProblemweb 10次 ==========");
        yield return StartCoroutine(StressTest_GETAllProblemwebCoroutine(10));

        yield return new WaitForSeconds(1f);

        // 第三轮：高并发GET 20次（10端点各2次）
        Debug.Log("========== 第3轮: 高并发GET 20次 ==========");
        StressTest_ConcurrentGet();
    }

    // 用于并行计数
    private int _loadCount = 0;
    private void IncrementLoad() { _loadCount++; }
    private void DecrementLoad() { _loadCount--; }

    // ==================== 数据字段 ====================
    // LoadAllData() 完成后可通过 SHSApiManager.字段 直接访问

    /// <summary>用户列表</summary>
    public static List<UserItem> UserList { get; private set; } = new List<UserItem>();

    /// <summary>题目列表（allProblemweb 返回的对象数组，非 DRF 分页格式）</summary>
    public static List<ProblemListItem> ProblemList { get; private set; } = new List<ProblemListItem>();

    /// <summary>题目详情字典（id → ProblemItem，按需查询）</summary>
    public static Dictionary<int, ProblemItem> ProblemDict { get; private set; } = new Dictionary<int, ProblemItem>();

    /// <summary>学校列表</summary>
    public static List<SchoolItem> SchoolList { get; private set; } = new List<SchoolItem>();

    /// <summary>班级列表</summary>
    public static List<ClassItem> ClassList { get; private set; } = new List<ClassItem>();

    /// <summary>课程列表</summary>
    public static List<CourseItem> CourseList { get; private set; } = new List<CourseItem>();

    /// <summary>章节列表</summary>
    public static List<ChapterItem> ChapterList { get; private set; } = new List<ChapterItem>();

    /// <summary>作业列表</summary>
    public static List<HomeworkItem> HomeworkList { get; private set; } = new List<HomeworkItem>();

    /// <summary>学生列表</summary>
    public static List<StudentItem> StudentList { get; private set; } = new List<StudentItem>();

    /// <summary>知识图谱列表</summary>
    public static List<KnowledgeGraphItem> KnowledgeGraphList { get; private set; } = new List<KnowledgeGraphItem>();

    /// <summary>作业缩略图列表</summary>
    public static List<WorkthumbnailItem> WorkthumbnailList { get; private set; } = new List<WorkthumbnailItem>();

    // ==================== 回调事件 ====================
    /// <summary>全部数据加载完成后触发</summary>
    public static event Action OnAllDataLoaded;

    // ==================== 内部 HTTP 工具（纯 UnityWebRequest，无外部依赖）====================

    private IEnumerator HttpGetRaw(string url, Action<string> onDone)
    {
        IncrementLoad();
        string result = null;
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                result = req.downloadHandler.text;
            }
            else
            {
                Debug.LogWarning($"[SHSApiManager] GET 失败 [{req.responseCode}]: {url}\n{req.error}");
                result = null;
            }
        }
        DecrementLoad();
        onDone?.Invoke(result);
    }

    private IEnumerator HttpPostRaw(string url, string jsonBody, Action<string> onDone)
    {
        IncrementLoad();
        string result = null;
        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.timeout = 10;
            req.SetRequestHeader("Content-Type", "application/json");
            req.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrEmpty(jsonBody))
                req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                result = req.downloadHandler.text;
            }
            else
            {
                Debug.LogWarning($"[SHSApiManager] POST FAIL [{req.responseCode}]: {url}\n{req.error}");
                result = null;
            }
        }
        DecrementLoad();
        onDone?.Invoke(result);  // 无论成功失败都必须调用回调，防止调用方超时死等
    }

    // ==================== 各 API 加载实现 ====================

    private IEnumerator LoadUserList()
    {
        string json = null;
        yield return HttpGetRaw(URL_UserSelect, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                var wrapper = JsonConvert.DeserializeObject<UserListResponse>(json);
                UserList = wrapper?.results ?? new List<UserItem>();
                Debug.Log($"[SHS] UserSelect 加载完成，共 {UserList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] UserSelect 解析失败: {ex.Message}\n原始JSON: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadProblemList()
    {
        string json = null;
        yield return HttpGetRaw(URL_allProblemweb, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                // allProblemweb 返回 [{"id":306,"title_id":"1-1-ht","course_id":1},...]
                var items = JsonConvert.DeserializeObject<List<ProblemListItem>>(json);
                ProblemList = items ?? new List<ProblemListItem>();
                Debug.Log($"[SHS] allProblemweb 加载完成，共 {ProblemList.Count} 个题目: {json.Substring(0, Mathf.Min(100, json.Length))}...");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] allProblemweb 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadSchoolList()
    {
        string json = null;
        yield return HttpGetRaw(URL_School, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                try { var w = JsonConvert.DeserializeObject<SchoolListResponse>(json); if (w?.results != null) { SchoolList = w.results; Debug.Log($"[SHS] School(DRF) 加载完成，共 {SchoolList.Count} 条"); yield break; } } catch { }
                var items = JsonConvert.DeserializeObject<List<SchoolItem>>(json);
                SchoolList = items ?? new List<SchoolItem>();
                Debug.Log($"[SHS] School(数组) 加载完成，共 {SchoolList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] School 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadClassList()
    {
        string json = null;
        yield return HttpGetRaw(URL_Classes, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                try { var w = JsonConvert.DeserializeObject<ClassListResponse>(json); if (w?.results != null) { ClassList = w.results; Debug.Log($"[SHS] Classes(DRF) 加载完成，共 {ClassList.Count} 条"); yield break; } } catch { }
                var items = JsonConvert.DeserializeObject<List<ClassItem>>(json);
                ClassList = items ?? new List<ClassItem>();
                Debug.Log($"[SHS] Classes(数组) 加载完成，共 {ClassList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] Classes 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadCourseList()
    {
        string json = null;
        yield return HttpGetRaw(URL_Course, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                try { var w = JsonConvert.DeserializeObject<CourseListResponse>(json); if (w?.results != null) { CourseList = w.results; Debug.Log($"[SHS] Course(DRF) 加载完成，共 {CourseList.Count} 条"); yield break; } } catch { }
                var items = JsonConvert.DeserializeObject<List<CourseItem>>(json);
                CourseList = items ?? new List<CourseItem>();
                Debug.Log($"[SHS] Course(数组) 加载完成，共 {CourseList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] Course 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadChapterList()
    {
        string json = null;
        yield return HttpGetRaw(URL_Chapter, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                try { var w = JsonConvert.DeserializeObject<ChapterListResponse>(json); if (w?.results != null) { ChapterList = w.results; Debug.Log($"[SHS] Chapter(DRF) 加载完成，共 {ChapterList.Count} 条"); yield break; } } catch { }
                var items = JsonConvert.DeserializeObject<List<ChapterItem>>(json);
                ChapterList = items ?? new List<ChapterItem>();
                Debug.Log($"[SHS] Chapter(数组) 加载完成，共 {ChapterList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] Chapter 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadHomeworkList()
    {
        string json = null;

        yield return HttpGetRaw(URL_Homework, s => json = s);

        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                try { var w = JsonConvert.DeserializeObject<HomeworkListResponse>(json); if (w?.results != null) { HomeworkList = w.results; Debug.Log($"[SHS] Homework(DRF) 加载完成，共 {HomeworkList.Count} 条"); yield break; } } catch { }
                var items = JsonConvert.DeserializeObject<List<HomeworkItem>>(json);
                HomeworkList = items ?? new List<HomeworkItem>();
                Debug.Log($"[SHS] Homework(数组) 加载完成，共 {HomeworkList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] Homework 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadKnowledgeGraph()
    {
        string json = null;
        yield return HttpGetRaw(URL_KnowledgeGraph, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                try { var w = JsonConvert.DeserializeObject<KnowledgeGraphListResponse>(json); if (w?.results != null) { KnowledgeGraphList = w.results; Debug.Log($"[SHS] KnowledgeGraph(DRF) 加载完成，共 {KnowledgeGraphList.Count} 条"); yield break; } } catch { }
                var items = JsonConvert.DeserializeObject<List<KnowledgeGraphItem>>(json);
                KnowledgeGraphList = items ?? new List<KnowledgeGraphItem>();
                Debug.Log($"[SHS] KnowledgeGraph(数组) 加载完成，共 {KnowledgeGraphList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] KnowledgeGraph 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    private IEnumerator LoadWorkthumbnail()
    {
        string json = null;
        yield return HttpGetRaw(URL_Workthumbnail, s => json = s);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                var wrapper = JsonConvert.DeserializeObject<WorkthumbnailListResponse>(json);
                WorkthumbnailList = wrapper?.results ?? new List<WorkthumbnailItem>();
                Debug.Log($"[SHS] Workthumbnail 加载完成，共 {WorkthumbnailList.Count} 条");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SHS] Workthumbnail 解析失败: {ex.Message}\n原始JSON前200字符: {json.Substring(0, Mathf.Min(200, json.Length))}");
            }
        }
    }

    // ==================== 手动刷新 ====================

    /// <summary>刷新用户列表</summary>
    public void RefreshUserList(Action<List<UserItem>> onDone)
    {
        StartCoroutine(DoRefresh(() => LoadUserList(), () => onDone?.Invoke(UserList)));
    }

    /// <summary>刷新题目列表</summary>
    public void RefreshProblemList(Action<List<ProblemListItem>> onDone)
    {
        StartCoroutine(DoRefresh(() => LoadProblemList(), () => onDone?.Invoke(ProblemList)));
    }

    /// <summary>刷新学校列表</summary>
    public void RefreshSchoolList(Action<List<SchoolItem>> onDone)
    {
        StartCoroutine(DoRefresh(() => LoadSchoolList(), () => onDone?.Invoke(SchoolList)));
    }

    /// <summary>刷新班级列表</summary>
    public void RefreshClassList(Action<List<ClassItem>> onDone)
    {
        StartCoroutine(DoRefresh(() => LoadClassList(), () => onDone?.Invoke(ClassList)));
    }

    private IEnumerator DoRefresh(Func<IEnumerator> loader, Action onDone)
    {
        yield return StartCoroutine(loader());
        onDone?.Invoke();
    }

    // ==================== 按需请求（POST 类）====================

    /// <summary>注册新用户 POST /back/UserRegisterModel/</summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <param name="email">邮箱（可选）</param>
    /// <param name="onDone">回调，参数为后端返回的 JSON 字符串</param>
    public void RegisterUser(string username, string password, string email, Action<string> onDone)
    {
        var body = new UserRegisterBody { username = username, password = password, email = email };
        string jsonBody = JsonConvert.SerializeObject(body);
        StartCoroutine(HttpPostRaw(URL_UserRegister, jsonBody, onDone));
    }

    /// <summary>添加学生 POST /back/StudentManagement/</summary>
    /// <param name="classId">班级ID</param>
    /// <param name="name">学生姓名</param>
    /// <param name="gender">性别（male/female）</param>
    /// <param name="onDone">回调，参数为后端返回的 JSON 字符串</param>
    public void AddStudent(int classId, string name, string gender, Action<string> onDone)
    {
        var body = new StudentAddBody { class_id = classId, name = name, gender = gender };
        string jsonBody = JsonConvert.SerializeObject(body);
        StartCoroutine(HttpPostRaw(URL_StudentManagement, jsonBody, onDone));
    }

    /// <summary>上传作业缩略图 POST /back/Workthumbnail/</summary>
    /// <param name="homeworkId">作业ID</param>
    /// <param name="thumbnailBase64">缩略图 Base64 字符串</param>
    /// <param name="studentId">学生ID</param>
    /// <param name="onDone">回调</param>
    public void UploadWorkthumbnail(int homeworkId, string thumbnailBase64, int studentId, Action<string> onDone)
    {
        var body = new WorkthumbnailUploadBody
        {
            homework = homeworkId,
            student = studentId,
            thumbnail = thumbnailBase64
        };
        string jsonBody = JsonConvert.SerializeObject(body);
        StartCoroutine(HttpPostRaw(URL_Workthumbnail, jsonBody, onDone));
    }

    // ==================== 数据模型 ====================
    // DRF 标准分页: { "count": N, "next": "...", "previous": "...", "results": [...] }

    #region ————— User —————

    [Serializable]
    public class UserListResponse { public int count; public string next; public string previous; public List<UserItem> results; }

    [Serializable]
    public class UserItem
    {
        public int id;
        public string username;
        public string password;      // 后端可能返回加密后的密码
        public string email;
        public string user_type;     // 身份类型: teacher / student / admin
        public int? school;         // 关联学校ID（可为空）
        public string created_at;   // 创建时间
    }

    [Serializable]
    public class UserRegisterBody
    {
        public string username;
        public string password;
        public string email;
    }

    #endregion

    #region ————— Problem —————

    [Serializable]
    public class ProblemListResponse { public int count; public string next; public string previous; public List<ProblemItem> results; }

    /// <summary>allProblemweb 返回的列表项（非 DRF 分页格式）</summary>
    [Serializable]
    public class ProblemListItem
    {
        public int id;
        public string title_id;   // 题号，如 "1-1-ht"
        public int course_id;      // 关联课程ID
    }

    [Serializable]
    public class ProblemItem
    {
        public int id;
        public string title;        // 题目名称/标题
        public string subject;      // 科目
        public string content;      // 题目内容（富文本/HTML）
        public int? difficulty;     // 难度等级 1-5
        public string answer;       // 正确答案
        public string analysis;     // 答案解析
        public string type;         // 题目类型: choice / fill / judge / subjective
        public int? score;          // 分值
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— School —————

    [Serializable]
    public class SchoolListResponse { public int count; public string next; public string previous; public List<SchoolItem> results; }

    [Serializable]
    public class SchoolItem
    {
        public int id;
        public string name;         // 学校名称
        public string address;      // 学校地址
        public string province;     // 省份
        public string city;         // 城市
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— Class —————

    [Serializable]
    public class ClassListResponse { public int count; public string next; public string previous; public List<ClassItem> results; }

    [Serializable]
    public class ClassItem
    {
        public int id;
        public string name;         // 班级名称
        public int school;          // 关联学校ID
        public string grade;        // 年级（如 "高一"）
        public int teacher;         // 班主任用户ID
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— Course —————

    [Serializable]
    public class CourseListResponse { public int count; public string next; public string previous; public List<CourseItem> results; }

    [Serializable]
    public class CourseItem
    {
        public int id;
        public string name;         // 课程名称
        public string description;  // 课程描述
        public int teacher;         // 授课教师用户ID
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— Chapter —————

    [Serializable]
    public class ChapterListResponse { public int count; public string next; public string previous; public List<ChapterItem> results; }

    [Serializable]
    public class ChapterItem
    {
        public int id;
        public int course;         // 关联课程ID
        public string title;        // 章节标题
        public int order;           // 章节序号
        public string description; // 章节描述
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— Homework —————

    [Serializable]
    public class HomeworkListResponse { public int count; public string next; public string previous; public List<HomeworkItem> results; }

    [Serializable]
    public class HomeworkItem
    {
        public int id;
        public string title;        // 作业标题
        public int course;          // 关联课程ID
        public int assigned_by;    // 布置人用户ID
        public string due_date;     // 截止日期（ISO 8601）
        public string description; // 作业描述
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— Student —————

    [Serializable]
    public class StudentItem
    {
        public int id;
        public string name;         // 学生姓名
        public int student_class;   // 关联班级ID（字段名 student_class 避免与 C# class 冲突）
        public string gender;       // 性别: male / female
        public string student_id;   // 学号
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— KnowledgeGraph —————

    [Serializable]
    public class KnowledgeGraphListResponse { public int count; public string next; public string previous; public List<KnowledgeGraphItem> results; }

    [Serializable]
    public class KnowledgeGraphItem
    {
        public int id;
        public string title;        // 知识点标题
        public string subject;      // 所属科目
        public string category;     // 分类
        public string relation;     // 关联关系
        public string created_at;   // 创建时间
    }

    #endregion

    #region ————— Workthumbnail —————

    [Serializable]
    public class WorkthumbnailListResponse { public int count; public string next; public string previous; public List<WorkthumbnailItem> results; }

    [Serializable]
    public class WorkthumbnailItem
    {
        public int id;
        public int homework;       // 关联作业ID
        public int student;         // 关联学生ID
        public string thumbnail;    // 缩略图（URL 或 Base64）
        public string status;       // 状态: pending / graded
        public string created_at;   // 上传时间
    }

    [Serializable]
    public class WorkthumbnailUploadBody
    {
        public int homework;
        public int student;
        public string thumbnail;
    }

    [Serializable]
    public class StudentAddBody
    {
        public int class_id;
        public string name;
        public string gender;
    }

    #endregion

    // ==================== 压力测试（仅调试用）====================

    [ContextMenu("SHS 诊断 — 服务器连通性")]
    public void DiagnoseServer()
    {
        StartCoroutine(DiagnoseServerCoroutine());
    }

    private IEnumerator DiagnoseServerCoroutine()
    {
        // 测试1：根路径 GET
        Debug.Log($"[诊断] GET {BASE_URL}/");
        bool done1 = false; string r1 = null;
        using (var req = UnityWebRequest.Get(BASE_URL + "/"))
        {
            req.timeout = 5;
            yield return req.SendWebRequest();
            r1 = req.result == UnityWebRequest.Result.Success
                ? $"OK ({req.responseCode}) {req.downloadHandler.text.Substring(0, Mathf.Min(200, req.downloadHandler.text.Length))}"
                : $"FAIL ({req.responseCode}) {req.error}";
            done1 = true;
        }
        Debug.Log($"[诊断] GET 结果: {r1}");

        // 测试2：POST RegisterUser
        Debug.Log($"[诊断] POST {URL_UserRegister}");
        bool done2 = false; string r2 = null;
        string body = JsonConvert.SerializeObject(new UserRegisterBody
        {
            username = $"diag_{System.DateTime.Now:HHmmss}",
            password = "diag_test",
            email = "diag@test.com"
        });
        Debug.Log($"[诊断] POST Body: {body}");
        using (var req = new UnityWebRequest(URL_UserRegister, "POST"))
        {
            req.timeout = 10;
            req.SetRequestHeader("Content-Type", "application/json");
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            r2 = req.result == UnityWebRequest.Result.Success
                ? $"OK ({req.responseCode}) {req.downloadHandler.text.Substring(0, Mathf.Min(200, req.downloadHandler.text.Length))}"
                : $"FAIL ({req.responseCode}) {req.error}";
            done2 = true;
        }
        Debug.Log($"[诊断] POST 结果: {r2}");
    }

    // ==================== 压力测试（GET 接收测试）====================

    /// <summary>
    /// 【压力测试】连续 GET 接收（UserSelect）
    /// </summary>
    [ContextMenu("SHS 压力测试 - GET UserSelect 10次")]
    public void StressTest_GETUserSelect()
    {
        StartCoroutine(StressTest_GETUserSelectCoroutine(10));
    }

    private IEnumerator StressTest_GETUserSelectCoroutine(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            int seq = i;
            Debug.Log($"[压测] #{i}/{count} GET {URL_UserSelect}");
            bool done = false;
            string response = null;

            StartCoroutine(HttpGetRaw(URL_UserSelect, s =>
            {
                response = s;
                done = true;
            }));

            float elapsed = 0f;
            while (!done && elapsed < 10f)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (done && !string.IsNullOrEmpty(response))
                Debug.Log($"[压测] GET UserSelect #{seq} OK 响应长度={response.Length} 前100字符: {response.Substring(0, Mathf.Min(100, response.Length))}");
            else
                Debug.LogError($"[压测] GET UserSelect #{seq} FAIL");

            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log($"[压测] GET UserSelect {count}次完成");
    }

    /// <summary>
    /// 【压力测试】连续 GET 接收（allProblemweb）
    /// </summary>
    [ContextMenu("SHS 压力测试 - GET allProblemweb 10次")]
    public void StressTest_GETAllProblemweb()
    {
        StartCoroutine(StressTest_GETAllProblemwebCoroutine(10));
    }

    private IEnumerator StressTest_GETAllProblemwebCoroutine(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            int seq = i;
            Debug.Log($"[压测] #{i}/{count} GET {URL_allProblemweb}");
            bool done = false;
            string response = null;

            StartCoroutine(HttpGetRaw(URL_allProblemweb, s =>
            {
                response = s;
                done = true;
            }));

            float elapsed = 0f;
            while (!done && elapsed < 10f)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (done && !string.IsNullOrEmpty(response))
                Debug.Log($"[压测] GET allProblemweb #{seq} OK 响应长度={response.Length} 前100字符: {response.Substring(0, Mathf.Min(100, response.Length))}");
            else
                Debug.LogError($"[压测] GET allProblemweb #{seq} FAIL");

            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log($"[压测] GET allProblemweb {count}次完成");
    }

    /// <summary>
    /// 【压力测试】高并发 GET（所有端点各2次，同时发出）
    /// </summary>
    [ContextMenu("SHS 压力测试 - 高并发GET 20次")]
    public void StressTest_ConcurrentGet()
    {
        var urls = new string[]
        {
            URL_UserSelect, URL_allProblemweb, URL_School, URL_Classes,
            URL_Course, URL_Chapter, URL_Homework, URL_StudentManagement,
            URL_KnowledgeGraph, URL_Workthumbnail
        };

        int total = urls.Length * 2;
        int finished = 0;
        int success = 0;
        int failed = 0;
        int seq = 0;

        foreach (string url in urls)
        {
            for (int round = 1; round <= 2; round++)
            {
                int roundSnapshot = round;
                seq++;
                int seqSnapshot = seq;

                Debug.Log($"[压测] >> 并发GET #{seqSnapshot}/{total} [{roundSnapshot}/2] {url}");
                bool done = false;
                string result = null;

                StartCoroutine(HttpGetRaw(url, s =>
                {
                    result = s;
                    done = true;
                }));

                StartCoroutine(MonitorRequest(seqSnapshot, total, () => done, () => result, r =>
                {
                    finished++;
                    if (r) success++;
                    else failed++;
                    string mark = r ? "OK" : "FAIL";
                    string len = r && !string.IsNullOrEmpty(result) ? $"长度{result.Length}" : "";
                    Debug.Log($"[压测] 并发GET #{seqSnapshot}/{total} [{roundSnapshot}/2] {mark} {len} ({finished}/{total} 成功{success} 失败{failed})");
                }));
            }
        }
        Debug.Log($"[压测] 高并发GET {total}个请求已全部发出，等待返回...");
    }

    private IEnumerator MonitorRequest(int index, int total, Func<bool> isDone, Func<string> getResult, Action<bool> onComplete)
    {
        float elapsed = 0f;
        while (!isDone() && elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        bool ok = isDone() && !string.IsNullOrEmpty(getResult());
        onComplete?.Invoke(ok);
    }
}

