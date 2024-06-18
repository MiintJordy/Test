using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPF_Study
{
    public class MyClass
    {
        private int m_nOne = 1;
        private int m_nTwo = 10;

        private string m_sOne = "일";
        private string m_sTwo = "십";
       public void Btn_Click_Func(object obj)
        {
            var Value = Add_Func(m_nOne, m_nTwo);
            Console.WriteLine(Value);
            var Value2 = Add_Func(m_sTwo, m_sOne);
            Console.WriteLine(Value2);
        }

        private T Add_Func<T>(T num1, T num2)
        {
            dynamic Result = num1;
            Result += num2;
            return Result; 
        }

        public ICommand Btn_Click
        {
            get
            {
                return new RelayCommand(Btn_Click_Func);
            }
        }
    }

    // ICommand 인터페이스 구현
    // 해당 이벤트가 발생하였을 때, RelayCommand 객체가 생성되고 실행이 끝나면 소멸한다고 생각하였음.
    // 하지만, 이벤트 발생 시 생성되고, 프로그램 종료 시까지 유지됨.
    // DataContext로 인해 상시 연결되므로 프로그램 실행과 동시에 객체가 생성되어있는 것처럼 보일 수 있음.
    public class RelayCommand : ICommand
    {
        // 명령이 실행될 때, 수행할 액션을 나타내는 private 읽기 전용 필드 선언
        private readonly Action<object> _execute;
        // 명령이 실행 가능한지 여부를 판단하는 조건을 나타내는 private 읽기 전용 필드 선언
        private readonly Predicate<object> _canExecute;

        // RelayCommand 클래스 생성자(매개변수: Action 대리자와 실행가능 여부 판단 대리자)
        // Action<Object> execute : 위 코드의 36번째 줄에 있는 Btn_Click_Func임.
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            // Action이 null이면 throw를 실행
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            // 실행 여부를 판단하는 대리자 할당
            _canExecute = canExecute;
        }

        // 실행 가능 여부 반환
        public bool CanExecute(object parameter)
        {
            Console.WriteLine("CanExeCute Param : " + parameter);
            Console.WriteLine("_canExecute : " + _canExecute);
            return _canExecute == null || _canExecute(parameter);
        }

        // 주어진 액션 실행
        public void Execute(object parameter)
        {
            Console.WriteLine("ExeCute Param : " + parameter);
            Console.WriteLine("_execute : " + _execute);
            _execute(parameter);
        }

        // UI 상태의 변경을 나타내는 이벤트가 발생함을 알림.
        public event EventHandler CanExecuteChanged
        {
            // 메서드와 연결된 핸들러를 실행함.
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // 작동 순서
        // 1. UI에서 이벤트 발생함.
        // 2. CanExecuteChanged가 UI 상태가 변경되는 이벤트가 발생함을 알림.
        // 3. 이벤트 발생에 대응하여 RequerySuggested에서 이벤트가 발생함을 메서드에 알림.
        // 4. CanExecuteChanged에 의해 _canExecute를 호출하여 실행 가능 여부 판단함.
        // 5. _canExecute 메서드의 결과에 따라 _execute 메서드를 호출함.
        // 6. 실행 가능하다면, Requery.Suggested를 호출하여 모든 핸들러 실행함.
    }
}
