"""Job Description Parser"""
import re
from typing import List, Dict
from dataclasses import dataclass

@dataclass
class JobData:
    """Structured job posting information"""
    required_skills: List[str]
    preferred_skills: List[str]
    experience_required: str
    education_required: str
    job_title: str
    raw_text: str

class JobParser:
    """Parse job description into structured data"""
    
    @staticmethod
    def extract_required_skills(text: str) -> List[str]:
        """Extract required skills from job description"""
        # Common tech skills
        skill_keywords = [
            'python', 'java', 'javascript', 'c\\+\\+', 'sql', 'nosql',
            'machine learning', 'deep learning', 'nlp', 'tensorflow',
            'pytorch', 'aws', 'azure', 'gcp', 'docker', 'kubernetes',
            'react', 'node.js', 'django', 'flask'
        ]
        
        text_lower = text.lower()
        found_skills = []
        
        for skill in skill_keywords:
            pattern = r'\b' + skill + r'\b'
            if re.search(pattern, text_lower):
                clean_skill = skill.replace('\\+\\+', '++')
                found_skills.append(clean_skill)
        
        return list(set(found_skills))
    
    @staticmethod
    def extract_preferred_skills(text: str) -> List[str]:
        """Extract preferred/nice-to-have skills"""
        pref_section = re.search(
            r'(?:preferred|nice to have|plus)(.*?)(?:benefits|$)',
            text,
            re.IGNORECASE | re.DOTALL
        )
        
        if pref_section:
            return JobParser.extract_required_skills(pref_section.group(1))
        return []
    
    @staticmethod
    def extract_experience_years(text: str) -> int:
        """Extract required years of experience"""
        patterns = [
            r'(\d+)\+?\s*years?\s*(?:of\s*)?experience',
            r'(\d+)\+?\s*yrs?\s*experience'
        ]
        
        for pattern in patterns:
            match = re.search(pattern, text, re.IGNORECASE)
            if match:
                return int(match.group(1))
        
        return 0
    
    @classmethod
    def parse(cls, job_text: str, job_title: str = "") -> JobData:
        """Parse job description into structured data"""
        return JobData(
            required_skills=cls.extract_required_skills(job_text),
            preferred_skills=cls.extract_preferred_skills(job_text),
            experience_required=str(cls.extract_experience_years(job_text)),
            education_required="",
            job_title=job_title,
            raw_text=job_text
        )