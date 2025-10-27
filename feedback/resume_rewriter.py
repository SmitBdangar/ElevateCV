"""Resume section rewriting utilities"""
import openai
import os
from dotenv import load_dotenv

load_dotenv()

class ResumeRewriter:
    """Rewrite resume sections for better impact"""
    
    def __init__(self, api_key: str = None):
        self.api_key = api_key or os.getenv('OPENAI_API_KEY')
        if self.api_key:
            openai.api_key = self.api_key
    
    def improve_bullet_point(self, original: str, skills_to_highlight: list) -> str:
        """Improve a single bullet point"""
        if not self.api_key:
            return f"• Improved version: {original}"
        
        prompt = f"""
Rewrite this resume bullet point to be more impactful:

Original: {original}

Requirements:
- Start with a strong action verb
- Include quantifiable results if possible
- Highlight these skills: {', '.join(skills_to_highlight[:3])}
- Keep it under 2 lines
- Make it ATS-friendly

Return only the improved bullet point, starting with "•"
"""
        
        try:
            response = openai.chat.completions.create(
                model="gpt-3.5-turbo",
                messages=[
                    {"role": "system", "content": "You are an expert resume writer."},
                    {"role": "user", "content": prompt}
                ],
                temperature=0.7,
                max_tokens=150
            )
            
            return response.choices[0].message.content.strip()
        except Exception as e:
            return f"• {original} (Error: {str(e)})"