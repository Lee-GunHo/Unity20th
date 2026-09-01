public interface IToolUsable
{
    void OnEquip(PlayerToolController owner);
    void OnUnequip();
    void Use(); // 단일 사용 메서드만 사용
}
