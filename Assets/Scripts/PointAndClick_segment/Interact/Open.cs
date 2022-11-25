using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open : InteractElement
{
    [SerializeField] private Animator _animator;
    [Header("��� ����� ����������� ���� 2 �����")]
    [SerializeField] private Open _open;
    public bool isLocked;
    public override void Use()
    {
        if (!isLocked)
        {
            OpenClose();
        }
    }

    protected virtual void OpenClose()
    {
        IsUsed = !IsUsed;

        if (_open != null)
        {
            _open.IsUsed = IsUsed;
        }

        if (IsUsed)
        {
            _animator.SetBool("Switch", true);
            Debug.Log("OpenAnimation");
        }
        else
        {
            _animator.SetBool("Switch", false);
            Debug.Log("CloseAnimation");
        }
    }
}
