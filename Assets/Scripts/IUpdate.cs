public interface IUpdate
{
	void ToUpdate(float deltaTime);

	bool IsFinish();

	void Finish();
}
