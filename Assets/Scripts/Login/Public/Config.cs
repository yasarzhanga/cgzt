using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    //58.56.66.165
    //192.168.1.182
    public static string IP { get; } = "58.56.66.165";

    //http://www.youerxiaoxuetang.com:12125
    static string localaddress = "http://192.168.10.193:8012";
    static string PubNetAddress = "http://58.56.66.177:8012";
    public static string ServerAddress { get; } = PubNetAddress;

    public static string LoginUrl { get; } = $"{ServerAddress}/back/Login/";
    /// <summary>
    /// ѧ����¼
    /// </summary>
    public static string LoginServerAddress { get; } = $"{ServerAddress}/back/Login/";
    /// <summary>
    /// ��ȡ����ʵ����Ϣ
    /// </summary>
    public static string TestAllInfo { get; } = $"{ServerAddress}/Experiment/";

    /// <summary>
    /// ����ʵ���¼post  ��ȡʵ���¼get �ύʵ�����
    /// </summary>
    public static string CreateTest { get; } = $"{ServerAddress}/ExperimentRecord/";
    /// <summary>
    /// ��ȡ���н׶�ģ����Ϣ  
    /// </summary>
    public static string ModelFile { get; } = $"{ServerAddress}/ModelFile/ExperimentModelInfo/";
    /// <summary>
    /// ��ȡһ���ж��ٸ�ʵ��׶�
    /// </summary>
    public static string ExperimentStage { get; } = $"{ServerAddress}/ExperimentStage/";
    /// <summary>
    /// ��¼ʵ����е��ĸ��׶�  ��ѯ���е��ĸ��׶�
    /// </summary>
    public static string ExperimentRecordInfo { get; } = $"{ServerAddress}/ExperimentRecordInfo/";
    /// <summary>
    /// ��¼ʵ����е��ĸ����� 
    /// </summary>
    public static string ExperimentRecordStep { get; } = $"{ServerAddress}/ExperimentRecordStep/";

    /// <summary>
    /// �ϴ�ģ�����ݵ�ĳ�׶�  �޸�ĳ�׶�ģ������  ɾ��ģ������  ��ѯģ������
    /// </summary>
    public static string ExperimentRecordInfoModel { get; } = $"{ServerAddress}/ExperimentRecordInfoModel/";
    /// <summary>
    /// �ϴ�����������ݵ�ĳ�׶�  ɾ�������������
    /// </summary>
    public static string ExperimentRecordInfoPoint { get; } = $"{ServerAddress}/ExperimentRecordInfoPoint/";
    /// <summary>
    /// �ϴ�ʵ������
    /// </summary>
    public static string ExperimentModelDetail { get; } = $"{ServerAddress}/ExperimentModelDetail/";
    /// <summary>
    /// �������
    /// </summary>
    public static string UserHeartbeat { get; } = $"{ServerAddress}/User/";

    public static string CameraRoaming { get; } = $"{ServerAddress}/CameraRoaming/";

    public static string StreamingAssetsPath { get; } = Application.streamingAssetsPath;

    //�˺����� 6-16λ���ֺ���ĸ������»��ߵ����
    public static string Admin { get; } = "^[0-9a-zA-Z_]{6,16}$";
    public static string MiMa { get; } = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$";
    /// <summary>
    /// ��֤��
    /// </summary>
    public static string VerificationCode { get; } = $"{ServerAddress}/back/image_codes/";
    /// <summary>
    /// �Ƿ��ѵ�¼
    /// </summary>
    public static string IsLogin { get; } = $"{ServerAddress}/back/isLogin/";
    /// <summary>
    /// �˳�
    /// </summary>
    public static string Exit { get; } = $"{ServerAddress}/back/Logout/";
    /// <summary>
    /// all��Ŀ����
    /// </summary>
    public static string GetProblemPartList { get; } = $"{ServerAddress}/back/Problemunity/getproblemlist/";
    /// <summary>
    /// ���ϴ�
    /// </summary>
    public static string AnswerUpLoad { get; } = $"{ServerAddress}/back/Answer/";
    /// <summary>
    /// �޸Ĵ𰸺��ϴ�
    /// </summary>
    public static string CorrectedAnswerUpLoad { get; } = $"{ServerAddress}/back/Answerunity/editAnswerwrongpoint/";
    /// <summary>
    /// �����ֻ��ŷ�����֤��
    /// </summary>
    public static string SendCodeByPhoneNumber { get; } = $"{ServerAddress}/back/Sendcode/";
    /// <summary>
    /// ��֤�ֻ��ź���֤��
    /// </summary>
    public static string CodeAndPhoneNumberVerifition { get; } = $"{ServerAddress}/back/CodeVerifition/";
    /// <summary>
    /// �޸�����
    /// </summary>
    public static string ChangePassword { get; } = $"{ServerAddress}/back/ChangePassword/";
    //�ֻ������� 11λ����
    public static string PhoneNum { get; } = @"^(1[3-9]\d{9}|999\d{1})$";// "^[0-9_]{11,11}$";
    /// <summary>
    /// ��Ŀ�ϴ�
    /// </summary>
    public static string UpLoadProblem { get; } = $"{ServerAddress}/back/Problem/";
    /// <summary>
    /// ��Ŀɾ��
    /// </summary>
    public static string DeleteProblem { get; } = $"{ServerAddress}/back/Problem/deleteproblemInfo/";
    /// <summary>
    /// ͨ����Դid��ȡ��Ŀ
    /// </summary>
    public static string GetSingleProblemInfo { get; } = $"{ServerAddress}/back/Problem/getsingleproblemInfo/";
    /// <summary>
    ///  /// <summary>
    /// ͨ����Ż�ȡ��Ŀ
    /// </summary>
    public static string GetSingleProblemInfoByTitleId { get; } = $"{ServerAddress}/back/Problem/titleidgetsingleproblemInfo/";
    /// <summary>
    /// ͨ����Ŀid��ȡ���д�
    /// </summary>
    public static string GetAllAnswerByProblemInfo { get; } = $"{ServerAddress}/back/Answerunity/getanswerlist/";
    /// <summary>
    /// ��ȡ������Ŀid
    /// </summary>
    public static string GetAllProblemID { get; } = $"{ServerAddress}/back/allProblemunity/";
    /// <summary>
    /// ����
    /// </summary>
    public static string GetHartBeat { get; } = $"{ServerAddress}/back/HeartBeat";
    /// <summary>
    /// У������ѧ���Ƿ���ڣ�GET��
    /// </summary>
    public static string GetCheckStudentExistence { get; } = $"{ServerAddress}/back/UserCard/check_student_existence/";

    /// <summary>
    /// ͨ��ѧ�Ż�ȡѧ��������GET��
    /// </summary>
    public static string GetStudentByStudentID { get; } = $"{ServerAddress}/back/StudentManagement/get_student_by_student_id/";

    /// <summary>
    /// ͨ���ֻ��Ż�ȡ��ʦ�������İ༶��GET��
    /// </summary>
    public static string GetManagedClassesByPhone { get; } = $"{ServerAddress}/back/StudentManagement/get_managed_classes_by_phone/";

    /// <summary>
    /// ͨ���༶ID��ȡѧ�������״̬��POST��
    /// </summary>
    public static string GetClassStudentsWithAnswerStatus { get; } = $"{ServerAddress}/back/StudentManagement/get_class_students_with_answer_status/";


}
