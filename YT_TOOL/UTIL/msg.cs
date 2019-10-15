using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YT_CAD_TOOL
{
    public class msg
    {
        public static void show(object message)
        {
            MessageBox.Show(message.ToString());
        }
        public static void show(params object[] messages)
        {
            var sb = new StringBuilder();
            messages.ToList().ForEach(m => sb.AppendLine(to.Str(m)));
            show(sb);
        }

        public static void Warning(string Message, string Caption)
        {
            MessageBoxButtons Button = MessageBoxButtons.OK;
            MessageBoxIcon Icon = MessageBoxIcon.Warning;

            MessageBox.Show(Message, Caption, Button, Icon);
        }

        public static void Information(string Message, string Caption)
        {
            MessageBoxButtons Button = MessageBoxButtons.OK;
            MessageBoxIcon Icon = MessageBoxIcon.Information;

            MessageBox.Show(Message, Caption, Button, Icon);
        }

        public static DialogResult Question(string Message, string Caption)
        {
            MessageBoxButtons Button = MessageBoxButtons.YesNo;
            MessageBoxIcon Icon = MessageBoxIcon.Question;

            return MessageBox.Show(Message, Caption, Button, Icon);
        }

        public static DialogResult Question2(string Message, string Caption)
        {
            MessageBoxButtons Button = MessageBoxButtons.YesNoCancel;
            MessageBoxIcon Icon = MessageBoxIcon.Question;

            return MessageBox.Show(Message, Caption, Button, Icon);
        }

        public class Is
        {
            public static DialogResult Delete()
            {
                var message = "선택 항목을 삭제하시겠습니까?";
                var caption = "항목삭제";

                return Question(message, caption);
            }

            public static DialogResult InitFile()
            {
                var message = new StringBuilder();
                message.AppendLine("현재 작업 중인 내용이 삭제됩니다.");
                message.AppendLine("그래도 초기화 하시겠습니까?");
                var caption = "초기화";

                return Question(message.ToString(), caption);
            }

            public static DialogResult Import()
            {
                var m = new StringBuilder();
                m.AppendLine("엑셀 가져오기를 하시겠습니까?");

                var c = "가져오기";

                return Question(m.ToString(), c);

            }

            public static DialogResult Save()
            {
                var message = "현재 도면의 작업 정보를 저장하시겠습니까?";
                var caption = "저장";

                return Question(message, caption);
            }

            public static DialogResult SaveOrCancle()
            {
                var message = "현재 도면의 작업 정보를 저장하시겠습니까?";
                var caption = "저장";

                return Question2(message, caption);
            }
        }

        public class Error
        {
            public static void Exist()
            {
                var message = "동일한 항목이 존재합니다.";
                var caption = "중복오류";

                var Button = MessageBoxButtons.OK;
                var Icon = MessageBoxIcon.Warning;

                MessageBox.Show(message, caption, Button, Icon);

            }

            public static void FileOpen()
            {
                var message = new StringBuilder();
                message.AppendLine("공간(실) 정보가 존재하지 않습니다.");
                message.AppendLine("Unit Factory를 이용하여 공간(실) 정보를 생성하세요");

                var caption = "오류";

                msg.Warning(message.ToString(), caption);
            }
        }

        public class Done
        {
            public static void ImportExcel()
            {
                var message = "엑셀 가져오기를 완료하였습니다.";
                var caption = "가져오기";

                msg.Information(message, caption);
            }

            public static void ExportExcel()
            {
                var message = "엑셀 내보내기를 완료하였습니다.";
                var caption = "내보내기";

                msg.Information(message, caption);
            }

            public static void ImportTemplete()
            {
                var message = "템플릿 파일 가져오기를 완료하였습니다.";
                var caption = "가져오기";

                msg.Information(message, caption);

            }

            public static void ExportTemplete()
            {
                var message = "템플릿 파일 내보내기를 완료하였습니다.";
                var caption = "내보내기";

                msg.Information(message, caption);
            }

            public static void Save()
            {
                var message = "현재 작업중인 내용이 저장되었습니다.";
                var caption = "저장";

                msg.Information(message, caption);
            }
        }
    }
}
