using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace jxzt {

    public class Info {
        /// <summary>
        /// ģ������
        /// </summary>
        [System.Serializable]
        public class ModelInfo {
            public string name;//ģ������
            public string md5;
            public string type;
            public float pos_x, pos_y, pos_h;
            public float rot_x, rot_y, rot_z;
            public float sca;
            public string ModelImg;//ģ��ͼƬ
            public string ModelColor;
            public int initId;//ģ�������н׶��е�id
            public int id;//����ģ�ͺ����������Ψһid
            public int ExperimentRecordStep;//���صĲ���id
        }

        /// <summary>
        /// ��¼����״̬��ѧ����Ϣ
        /// </summary>

        public class ReturnState {
            public int code;//״̬��
            public int id;//�û�id
            public string message;//��Ϣ
            public string username;//�û���
            public StudentInfo data;
            public int attempts;//����ʣ�����
            public int Identitypermissions;//��¼���� 1��������2����ʦ��3��ѧ����
        }
        /// <summary>
        /// ��¼��ѧ����Ϣ
        /// </summary>
        [System.Serializable]
        public class StudentInfo {
            public int id;//ѧ��id
            public string TypeDetail;//���ͺ���
            public string UserName;//ѧ������
            public int CardID;//�˺�id
            public string Gender;//�Ա�
            public string PhoneNum;//�绰
            public string CreateTime;//�˺Ŵ���ʱ��
            public string UpdateTime;//�˺����ʱ��
            public int UserType;//��������
            public int Classify;//�༶id
            public int attempts;//����ʣ�����
        }
        //�ӿ�ͨ����
        public class Connect<T> {
            public int code;
            public string message;
            public T data;
        }
        public class ReturnStateTeacher {
            public int code;//״̬��
            public string message;//��Ϣ              
            public singletitleData[] data;//������Ŀ           
            public int id;//�û�id
            public response response;
            public ReturnStateTeacher(singletitleData[] data) {
                this.data = data;
            }

        }
        public class ReturnStudentMessageIsExist {
            public int code;//״̬��
            public string message;//��Ϣ              
            public bool data;//�Ƿ����                                

        }
        public class ReturnAllTitleDataTeacher : Connect<titleData> {
            public int sum;//��Ŀ����
            public titleData[] allTitleData;//��Ŀ


        }
        public class ReturnAllStudentAnswerDataByTitle : Connect<StudentAnswerDataByTitle> {
            public int sum;//������
            public StudentAnswerDataByTitle[] data;//��Ŀ


        }
        public class ReturnAllTitleIdData : Connect<titleIdData> {
            public titleIdData[] allTitleIdData;//������ĿID
        }
        [System.Serializable]
        public class TeacherInfo {
            public int id;//��ʦid
            public string TypeDetail;//���ͺ���
            public int CardID;//�˺�id
            public string TeacherName;//��ʦ����
            public string CreateTime;//�˺Ŵ���ʱ��
            public int UserType;//��������
        }
        [System.Serializable]
        public class response {
            public int code;//״̬��
            public string status;//״̬��Ϣ            
        }


        /// <summary>
        /// ��ҳ����
        /// </summary>
        public class Page {
            public int count;//һ������������
            public string next;//��һҳ�������ַ ���������Ϊnull
            public string previous;//��һҳ�������ַ ���������Ϊnull
            public List<TestInfo> results;//��ҳ��ʵ����Ϣ
        }

        /// <summary>
        /// ����ʵ����Ϣ
        /// </summary>
        public class TestInfo {
            public int id;//ʵ��id
            public string ExperimentName;//ʵ������
            public string CreateTime;//����ʱ��
            public int Teacher;//�����ߵ�id
            public string SceneModel;//ģ��ip
            public string SceneImg;//ͼƬip
            public string FileMD5Code;//md5
        }

        /// <summary>
        /// ��ҳ���� �Ѿ���ʼ��ʵ��
        /// </summary>
        public class Page2 {
            public int count;//һ������������
            public string next;//��һҳ�������ַ ���������Ϊnull
            public string previous;//��һҳ�������ַ ���������Ϊnull
            public List<ReturnTestInfo> results;//��ҳ��ʵ����Ϣ
        }

        /// <summary>
        /// get����ʵ���ķ�����Ϣ  post��ѯʵ���ķ�����Ϣ
        /// </summary>
        [System.Serializable]
        public class ReturnTestInfo {
            public int id;//��¼id
            public string ExperimentName;//ʵ������
            public string RecordImg;//ʵ��ͼƬ  ǰ����Ҫ�� IP����ƴ�ӵ�ַ
            public string FileMD5Code;//MD5
            public float Score;//ʵ�����
            public bool IsFinish;//�Ƿ����
            public string CreateTime;//ʵ�鴴��ʱ��
            public string UpdateTime;//ʵ�����ʱ��
            public int Experiment;//ʵ���id
            public int User;//ѧ��id
            public string StudentName;//ѧ������
            public string SceneModel;//ģ�����ص�ַ
        }

        /// <summary>
        /// ���н׶ε�ģ������
        /// </summary>

        [System.Serializable]
        public class ModelChildInfo {
            public int id;//ģ��id �����ύ�ɼ�
            public string[] StandardDetail;//ģ�Ͷ�����Ϣ
            public string ModelName;//ģ������
            public string File;//ģ�����ص�ַ
            public string FileMD5Code;//MD5
            public string ModelImg;//ģ��ͼƬ
            public string ModelStandardDetail;//ģ�Ͷ�����Ϣ
            public string CreateTime;//����ʱ��
            public int ExperimentStep;//����
        }

        /// <summary>
        /// ģ�͵����ص�ַ������
        /// </summary>
        [System.Serializable]
        public class ModelDownIPAndName {
            public string ip;
            public string name;
            public string md5;
        }

        /// <summary>
        /// ģ��һ���ж��ٽ׶ζ��ٲ���  (���󵽵�����)   
        /// </summary>
        public class ModelJieDuanInfo {
            public int id;//�׶�id ͨ������ҵ��˽׶ε�ģ����Ϣ
            public List<BuZhou> StepInfo;
            public string StageName;//�׶�����
            public string CreateTime;//����ʱ��
            public int Experiment;//ʵ��id
        }
        [System.Serializable]
        public class BuZhou {
            public int id;//����id
            public string StepName;//��������
            public string CreateTime;//�����¼�
            public int ExperimentStage;//�׶�id
        }
        /// <summary>
        /// ģ��һ�����ٽ׶� ���޸ĺ� ����д���룩
        /// </summary>
        [System.Serializable]
        public class ModeJieDuanInfoUpdate {
            public int id;//�׶�id ͨ������ҵ��˽׶ε�ģ����Ϣ
            public string StageName;//�׶�����
            public string CreateTime;//����ʱ��
            public int Experiment;//��С���� �׶�˳��
            public int index;//ʵ���±�
            public List<BuZhou> StepInfo;
        }

        /// <summary>
        /// �ϴ��׶���Ϣ(ֻ��������)
        /// </summary>
        public class UpJieDuan {
            public int id;//ʵ���¼id
            public int Experiment;//�׶�id
        }

        /// <summary>
        /// ��ǰʵ����еĽ׶���Ϣ  �����洢ģ����Ϣ
        /// </summary>
        [System.Serializable]
        public class UpJieDuanRequest {
            public int id;//���صĽ׶�id
            public string CreateTime;
            public int ExperimentRecord;//ʵ���¼id
            public int ExperimentStage;//�׶�id
            public int index;//�׶��±�
            public List<UpStepRequest> RecordStepInfo;
        }
        /// <summary>
        /// ��ǰʵ����еĲ�����Ϣ 
        /// </summary>
        [System.Serializable]
        public class UpStepRequest {
            public int id;//���صĲ���id
            public string CreateTime;
            public int ExperimentRecordInfo;//���صĽ׶�id
            public int ExperimentStep;//����id
            public int index;//�����±�
        }
        /// <summary>
        /// �ϴ�ģ�����ݺ�ķ������� 
        /// </summary>
        public class UpJieDuanModelRequestInfo {
            public int id;//�˽׶���ģ������id  Ψһ
            public string ModelName;//ģ������
            public string ModelID;//ģ�������н׶��е�id
            public string Position;//ģ������
            public string Rotation;//ģ�ͽǶ�
            public string Scale;//ģ������
            public string ModelColor;//ģ����ɫ
            public int ExperimentRecordStep;//Ҫ�ϴ���ĳ�������id
        }

        public class PointInfo {
            public int id;//�������߱�����ݵ�id
            public string PointPos;
            public string CreateTime;
            public string ExperimentRecordStep;//Ҫ�ϴ���ĳ�������id
        }


        /// <summary>
        /// ���η��ص�����
        /// </summary>
        public class Roaming {
            public int id;
            public int ExperimentRecord;
            public string PathList;

        }
        /// <summary>
        /// �洢�������������� �Զ���
        /// </summary>
        public class Paths {
            public int speed;//�����ٶ�
            public List<string> nameList = new List<string>();//·��������
            public List<string> pathList = new List<string>();//·������json
        }

        /// <summary>
        /// �ϴ�����ʷ����
        /// </summary>
        public class SubmitInfo 
        {
            public int id;//��id ûɶ��
            public string JsonDetail;//��������json ת �ֵ�
            public int ModelFile;//ģ��id
            public int ExperimentRecord;//ʵ���¼id
        }
        /// <summary>
        /// ��Ŀ����
        /// </summary>


        public class ProblemPartList {
            public titleData[] data;

            public ProblemPartList(titleData[] data) {
                this.data = data;
            }
        }
        public class titleData {
            public int id;//id
            public string title;//��ĿͼƬ
            public string title_id;//��Ŀ���
            public string AreaResource;//��Դid
            public string StandardAnswer;//��׼��url
        }
        public class titleIdData {
            public int id;//id            
            public string title_id;//��Ŀ���
            public string AreaResource;//��Դid


        }
        public class singletitleData {
            public int id;//id
            public string title_id;//��Ŀ���
            public string title;//��ĿͼƬurl
            public string StandardAnswer;//��׼��url
            public string AreaResource;//��Դid
        }
        public class StudentAnswerDataByTitle {
            public int id;//id
            public string username;//����
            public string userclasses;//�༶
            public string userstudentid;//ѧ��
            public string PersonalAnswer;//���˴�ͼƬurl
            public string WrongPoint;//�����
        }
        /// <summary>
        /// ��д����ʶ�𷵻�����
        /// </summary>        
        public class HandwritingOcr {
            /// <summary>
            /// ʶ����������ʾwords_result��Ԫ�ظ���
            /// </summary>
            public int words_result_num;
            /// <summary>
            /// Ψһ��log id���������ⶨλ
            /// </summary>
            public long log_id;
            /// <summary>
            /// ��λ��ʶ��������
            /// </summary>
            public Words_Result[] words_result;
            /// <summary>
            /// ͼ���򣬵�detect_direction=trueʱ���ڣ���ʾͼƬ�ķ���
            /// </summary>
            public int direction;

        }
        /// <summary>
        /// ����ʶ���ά��ʶ�𷵻�����
        /// </summary>        
        public class OcrQRCode {
            /// <summary>
            /// ʶ����������ʾwords_result��Ԫ�ظ���
            /// </summary>
            public int codes_result_num;
            /// <summary>
            /// Ψһ��log id���������ⶨλ
            /// </summary>
            public long log_id;
            /// <summary>
            /// ��λ��ʶ��������
            /// </summary>
            public codes_result[] codes_result;

        }
        /// <summary>
        ///��λ��ʶ����
        /// </summary>        
        public class Words_Result {
            /// <summary>
            /// ��λ��ʶ��������
            /// </summary>
            public Location location;
            /// <summary>
            /// ʶ�����ַ���
            /// </summary>
            public string words;
        }
        /// <summary>
        ///��λ��ʶ����
        /// </summary>        
        public class codes_result {
            /// <summary>
            /// ��λ��ʶ��������
            /// </summary>
            public string text;
            /// <summary>
            /// ʶ�����ַ���
            /// </summary>
            public string type;
        }
        /// <summary>
        /// �����Ŷ���Ϣ
        /// </summary>        
        public class Location {
            /// <summary>
            /// ��ʾ��λλ�õĳ��������϶����ˮƽ����
            /// </summary>
            public int left;
            /// <summary>
            /// ��ʾ��λλ�õĳ��������϶���Ĵ�ֱ����
            /// </summary>
            public int top;
            /// <summary>
            /// ��ʾ��λ��λλ�õĳ����εĿ���
            /// </summary>
            public int width;
            /// <summary>
            /// ��ʾλ�õĳ����εĸ߶�
            /// </summary>
            public int height;
        }


        public class UpClassRequestInfo {
            public List<int> class_ids;
            public List<int> problem_ids;

            public UpClassRequestInfo(List<int> class_ids, List<int> problem_ids) {
                this.class_ids = class_ids;
                this.problem_ids = problem_ids;
            }
        }

        public class ReturnStudentInfoList {
            public int code;
            public string message;
            public List<Data> data;
            public class Data {
                public int user_id;
                public string student_id;
                public string student_name;
                public string class_name;
                public int class_id;
                public string school_name;
                public Dictionary<string, bool> answer_status;
            }
        }

        public class ReturnStudentInfo {
            public int code;
            public string message;
            public Data data;

            public class Data {
                public int user_id;
                public string student_id;
                public string student_name;
                public string class_name;
                public int class_id;
                public string school_name;
            }
        }

        public class ReturnClassList {
            public int code;
            public string message;
            public Data data;
            public class Data {
                public TeacherInfo user_info;
                public List<ClassInfo> managed_classes;

                public class TeatherInfo {
                    public int user_id;
                    public string phone;
                    public string user_name;
                    public int identity;
                    public string identity_name;

                }

                public class ClassInfo {
                    public int id;
                    public string name;
                    public string school_name;
                    public int school_id;
                    public string teacher_name;
                    public int teacher_id;
                }
            }
        }

        public class ReturnAIImageInfo {
            public string id;
            public string name;
            public int size;
            public string extension;
            public string mime_type;
            public string created_by;
            public int created_at;

        }

        public class AIMassageInfo {
            public string query;
            public string user;
            public string response_mode = "blocking";
            public string inputs = null;

        }
    }
}